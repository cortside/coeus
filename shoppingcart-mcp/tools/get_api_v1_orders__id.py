from opentelemetry.trace import Status, StatusCode
from utils.tracing import get_tracer
from app.mcp_instance import mcp
from utils.client import call_api
from utils.metrics import tool_invocations_total, tool_validation_errors_total
from .get_api_v1_orders__id_models import get_api_v1_orders__id_Input, get_api_v1_orders__id_Output
from utils.output_helper import create_output
from typing import get_args
tracer = get_tracer("acme.shoppingcart.mcp.tool.get_api_v1_orders__id")
@mcp.tool(name="get_api_v1_orders__id", description="Gets an order by id (Auth permission: GetOrder)", tags={"route:/api/v1/orders/{id}", "method:GET"})
async def get_api_v1_orders__id(input: get_api_v1_orders__id_Input) -> get_api_v1_orders__id_Output:
    tool_invocations_total.labels(tool="get_api_v1_orders__id", outcome="started").inc()
    query_params = {}
    try:
        from pydantic import ValidationError
        with tracer.start_as_current_span("tool_call") as span:
            span.set_attribute("tool.name", "get_api_v1_orders__id")
            res = await call_api("GET", f"/api/v1/orders/{input.id}", params=query_params, tool="get_api_v1_orders__id")
            tool_invocations_total.labels(tool="get_api_v1_orders__id", outcome="success").inc()
            return create_output(get_api_v1_orders__id_Output, res)
    except ValidationError:
        tool_validation_errors_total.labels(tool="get_api_v1_orders__id").inc()
        return get_api_v1_orders__id_Output()
    except Exception:
        tool_invocations_total.labels(tool="get_api_v1_orders__id", outcome="error").inc()
        return get_api_v1_orders__id_Output()
