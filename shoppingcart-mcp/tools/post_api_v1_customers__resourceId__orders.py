from opentelemetry.trace import Status, StatusCode
from utils.tracing import get_tracer
from app.mcp_instance import mcp
from utils.client import call_api
from utils.metrics import tool_invocations_total, tool_validation_errors_total
from .post_api_v1_customers__resourceId__orders_models import post_api_v1_customers__resourceId__orders_Input, post_api_v1_customers__resourceId__orders_Output
from utils.output_helper import create_output
from typing import get_args
tracer = get_tracer("acme.shoppingcart.mcp.tool.post_api_v1_customers__resourceId__orders")
@mcp.tool(name="post_api_v1_customers__resourceId__orders", description="Create a new order (Auth permission: CreateOrder)", tags={"route:/api/v1/customers/{resourceId}/orders", "method:POST"})
async def post_api_v1_customers__resourceId__orders(input: post_api_v1_customers__resourceId__orders_Input) -> post_api_v1_customers__resourceId__orders_Output:
    tool_invocations_total.labels(tool="post_api_v1_customers__resourceId__orders", outcome="started").inc()
    query_params = {}
    try:
        from pydantic import ValidationError
        with tracer.start_as_current_span("tool_call") as span:
            span.set_attribute("tool.name", "post_api_v1_customers__resourceId__orders")
            res = await call_api("POST", f"/api/v1/customers/{input.resourceId}/orders", params=query_params, json=input.body, tool="post_api_v1_customers__resourceId__orders")
            tool_invocations_total.labels(tool="post_api_v1_customers__resourceId__orders", outcome="success").inc()
            return create_output(post_api_v1_customers__resourceId__orders_Output, res)
    except ValidationError:
        tool_validation_errors_total.labels(tool="post_api_v1_customers__resourceId__orders").inc()
        return post_api_v1_customers__resourceId__orders_Output()
    except Exception:
        tool_invocations_total.labels(tool="post_api_v1_customers__resourceId__orders", outcome="error").inc()
        return post_api_v1_customers__resourceId__orders_Output()
