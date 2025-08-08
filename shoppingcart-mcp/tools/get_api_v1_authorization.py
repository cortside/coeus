from opentelemetry.trace import Status, StatusCode
from utils.tracing import get_tracer
from app.mcp_instance import mcp
from utils.client import call_api
from utils.metrics import tool_invocations_total, tool_validation_errors_total
from .get_api_v1_authorization_models import get_api_v1_authorization_Input, get_api_v1_authorization_Output
from utils.output_helper import create_output
from typing import get_args
tracer = get_tracer("acme.shoppingcart.mcp.tool.get_api_v1_authorization")
@mcp.tool(name="get_api_v1_authorization", description="Gets the list if permissions associated with the caller, determined by their bearer token (Auth)", tags={"route:/api/v1/authorization", "method:GET"})
async def get_api_v1_authorization(input: get_api_v1_authorization_Input) -> get_api_v1_authorization_Output:
    tool_invocations_total.labels(tool="get_api_v1_authorization", outcome="started").inc()
    query_params = {}
    try:
        from pydantic import ValidationError
        with tracer.start_as_current_span("tool_call") as span:
            span.set_attribute("tool.name", "get_api_v1_authorization")
            res = await call_api("GET", f"/api/v1/authorization", params=query_params, tool="get_api_v1_authorization")
            tool_invocations_total.labels(tool="get_api_v1_authorization", outcome="success").inc()
            return create_output(get_api_v1_authorization_Output, res)
    except ValidationError:
        tool_validation_errors_total.labels(tool="get_api_v1_authorization").inc()
        return get_api_v1_authorization_Output()
    except Exception:
        tool_invocations_total.labels(tool="get_api_v1_authorization", outcome="error").inc()
        return get_api_v1_authorization_Output()
