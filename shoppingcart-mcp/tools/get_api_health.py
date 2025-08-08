from opentelemetry.trace import Status, StatusCode
from utils.tracing import get_tracer
from app.mcp_instance import mcp
from utils.client import call_api
from utils.metrics import tool_invocations_total, tool_validation_errors_total
from .get_api_health_models import get_api_health_Input, get_api_health_Output
from schemas.models import Cortside_Health_Models_HealthModel
from utils.output_helper import create_output
tracer = get_tracer("acme.shoppingcart.mcp.tool.get_api_health")
@mcp.tool(name="get_api_health", description="get_api_health", tags={"route:/api/health", "method:GET"})
async def get_api_health(input: get_api_health_Input) -> get_api_health_Output:
    tool_invocations_total.labels(tool="get_api_health", outcome="started").inc()
    query_params = {}
    try:
        from pydantic import ValidationError
        with tracer.start_as_current_span("tool_call") as span:
            span.set_attribute("tool.name", "get_api_health")
            res = await call_api("GET", f"/api/health", params=query_params, tool="get_api_health")
            tool_invocations_total.labels(tool="get_api_health", outcome="success").inc()
            return create_output(get_api_health_Output, res)
    except ValidationError:
        tool_validation_errors_total.labels(tool="get_api_health").inc()
        return get_api_health_Output()
    except Exception:
        tool_invocations_total.labels(tool="get_api_health", outcome="error").inc()
        return get_api_health_Output()
