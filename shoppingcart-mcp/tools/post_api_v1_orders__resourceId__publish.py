from opentelemetry.trace import Status, StatusCode
from utils.tracing import get_tracer
from app.mcp_instance import mcp
from utils.client import call_api
from utils.metrics import tool_invocations_total, tool_validation_errors_total
from utils.output_helper import create_output
from .post_api_v1_orders__resourceId__publish_models import post_api_v1_orders__resourceId__publish_Input, post_api_v1_orders__resourceId__publish_Output
from typing import get_args
tracer = get_tracer("acme.shoppingcart.mcp.tool.post_api_v1_orders__resourceId__publish")
@mcp.tool(name="post_api_v1_orders__resourceId__publish", description="Update an order (Auth permission: PublishOrder)", tags={"route:/api/v1/orders/{resourceId}/publish", "method:POST"})
async def post_api_v1_orders__resourceId__publish(input: post_api_v1_orders__resourceId__publish_Input) -> post_api_v1_orders__resourceId__publish_Output:
    tool_invocations_total.labels(tool="post_api_v1_orders__resourceId__publish", outcome="started").inc()
    query_params = {}
    try:
        from pydantic import ValidationError
        with tracer.start_as_current_span("tool_call") as span:
            span.set_attribute("tool.name", "post_api_v1_orders__resourceId__publish")
            res = await call_api("POST", f"/api/v1/orders/{input.resourceId}/publish", params=query_params, tool="post_api_v1_orders__resourceId__publish")
            tool_invocations_total.labels(tool="post_api_v1_orders__resourceId__publish", outcome="success").inc()
            return create_output(post_api_v1_orders__resourceId__publish_Output, res)
    except ValidationError:
        tool_validation_errors_total.labels(tool="post_api_v1_orders__resourceId__publish").inc()
        return post_api_v1_orders__resourceId__publish_Output()
    except Exception:
        tool_invocations_total.labels(tool="post_api_v1_orders__resourceId__publish", outcome="error").inc()
        return post_api_v1_orders__resourceId__publish_Output()
