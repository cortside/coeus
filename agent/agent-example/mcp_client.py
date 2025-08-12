import httpx
import asyncio
import json
import logging
from typing import Any, Dict, Optional
from pydantic import BaseModel, ValidationError
from prometheus_client import Counter, Histogram
from opentelemetry import trace

logger = logging.getLogger(__name__)
tracer = trace.get_tracer(__name__)

# Prometheus metrics
TOOL_CALL_COUNTER = Counter(
    "mcp_tool_calls_total",
    "Total MCP tool calls",
    ["tool_name", "status"]
)
TOOL_CALL_DURATION = Histogram(
    "mcp_tool_call_duration_seconds",
    "Duration of MCP tool calls",
    ["tool_name"]
)

class ToolOutputSchema(BaseModel):
    result: Any

class MCPClient:
    def __init__(self, base_url: str, timeout: float = 10.0, max_retries: int = 3):
        self.base_url = base_url.rstrip("/")
        self.timeout = timeout
        self.max_retries = max_retries

    async def call_tool(
        self,
        tool_name: str,
        payload: Dict[str, Any],
        output_schema: Optional[BaseModel] = None
    ) -> Dict[str, Any]:
        attempt = 0
        last_error = None

        async with httpx.AsyncClient(timeout=self.timeout) as client:
            while attempt < self.max_retries:
                attempt += 1
                try:
                    with tracer.start_as_current_span(f"mcp.{tool_name}") as span:
                        span.set_attribute("mcp.tool_name", tool_name)
                        span.set_attribute("mcp.attempt", attempt)

                        logger.info(f"Calling MCP tool {tool_name}, attempt {attempt}")

                        with TOOL_CALL_DURATION.labels(tool_name).time():
                            resp = await client.post(
                                f"{self.base_url}/tools/{tool_name}",
                                json=payload
                            )

                        if resp.status_code != 200:
                            raise RuntimeError(
                                f"Tool {tool_name} failed with status {resp.status_code}"
                            )

                        data = resp.json()

                        if output_schema:
                            try:
                                data = output_schema.parse_obj(data).dict()
                            except ValidationError as ve:
                                logger.error(f"Schema validation failed: {ve}")
                                raise

                        TOOL_CALL_COUNTER.labels(tool_name, "success").inc()
                        return data

                except (httpx.RequestError, RuntimeError, ValidationError) as e:
                    last_error = e
                    logger.warning(
                        f"Error calling MCP tool {tool_name} (attempt {attempt}): {e}"
                    )
                    TOOL_CALL_COUNTER.labels(tool_name, "error").inc()

                    if attempt < self.max_retries:
                        await asyncio.sleep(2 ** attempt)

            logger.error(f"Tool {tool_name} failed after {self.max_retries} attempts")
            raise last_error
