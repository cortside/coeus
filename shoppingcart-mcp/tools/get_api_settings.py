from opentelemetry.trace import Status, StatusCode
from utils.tracing import get_tracer
from app.mcp_instance import mcp
from utils.client import call_api
from utils.metrics import tool_invocations_total, tool_validation_errors_total
from .get_api_settings_models import get_api_settings_Input, get_api_settings_Output
from utils.output_helper import create_output
from typing import get_args
tracer = get_tracer("acme.shoppingcart.mcp.tool.get_api_settings")
@mcp.tool(name="get_api_settings", description="Service settings that a consumer may need to be aware of", tags={"route:/api/settings", "method:GET"})
async def get_api_settings(input: get_api_settings_Input) -> get_api_settings_Output:
    tool_invocations_total.labels(tool="get_api_settings", outcome="started").inc()
    query_params = {}
    try:
        from pydantic import ValidationError
        with tracer.start_as_current_span("tool_call") as span:
            span.set_attribute("tool.name", "get_api_settings")
            res = await call_api("GET", f"/api/settings", params=query_params, tool="get_api_settings")
            tool_invocations_total.labels(tool="get_api_settings", outcome="success").inc()
            return create_output(get_api_settings_Output, res)
    except ValidationError:
        tool_validation_errors_total.labels(tool="get_api_settings").inc()
        return get_api_settings_Output()
    except Exception:
        tool_invocations_total.labels(tool="get_api_settings", outcome="error").inc()
        return get_api_settings_Output()
