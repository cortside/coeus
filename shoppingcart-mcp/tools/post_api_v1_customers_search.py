from opentelemetry.trace import Status, StatusCode
from utils.tracing import get_tracer
from app.mcp_instance import mcp
from utils.client import call_api
from utils.metrics import tool_invocations_total, tool_validation_errors_total
from .post_api_v1_customers_search_models import post_api_v1_customers_search_Input, post_api_v1_customers_search_Output
from utils.output_helper import create_output
from typing import get_args
tracer = get_tracer("acme.shoppingcart.mcp.tool.post_api_v1_customers_search")
@mcp.tool(name="post_api_v1_customers_search", description="Gets customers by post (Auth permission: GetCustomers)", tags={"route:/api/v1/customers/search", "method:POST"})
async def post_api_v1_customers_search(input: post_api_v1_customers_search_Input) -> post_api_v1_customers_search_Output:
    tool_invocations_total.labels(tool="post_api_v1_customers_search", outcome="started").inc()
    query_params = {}
    try:
        from pydantic import ValidationError
        with tracer.start_as_current_span("tool_call") as span:
            span.set_attribute("tool.name", "post_api_v1_customers_search")
            res = await call_api("POST", f"/api/v1/customers/search", params=query_params, json=input.body, tool="post_api_v1_customers_search")
            tool_invocations_total.labels(tool="post_api_v1_customers_search", outcome="success").inc()
            return create_output(post_api_v1_customers_search_Output, res)
    except ValidationError:
        tool_validation_errors_total.labels(tool="post_api_v1_customers_search").inc()
        return post_api_v1_customers_search_Output()
    except Exception:
        tool_invocations_total.labels(tool="post_api_v1_customers_search", outcome="error").inc()
        return post_api_v1_customers_search_Output()
