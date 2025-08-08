from opentelemetry.trace import Status, StatusCode
from utils.tracing import get_tracer
from app.mcp_instance import mcp
from utils.client import call_api
from utils.metrics import tool_invocations_total, tool_validation_errors_total
from .get_api_v1_customers_models import get_api_v1_customers_Input, get_api_v1_customers_Output
from utils.output_helper import create_output
from typing import get_args
tracer = get_tracer("acme.shoppingcart.mcp.tool.get_api_v1_customers")
@mcp.tool(name="get_api_v1_customers", description="Gets customers (Auth permission: GetCustomers)", tags={"route:/api/v1/customers", "method:GET"})
async def get_api_v1_customers(input: get_api_v1_customers_Input) -> get_api_v1_customers_Output:
    tool_invocations_total.labels(tool="get_api_v1_customers", outcome="started").inc()
    query_params = {}
    if getattr(input, "CustomerResourceId") is not None:
        query_params["CustomerResourceId"] = getattr(input, "CustomerResourceId")
    if getattr(input, "FirstName") is not None:
        query_params["FirstName"] = getattr(input, "FirstName")
    if getattr(input, "LastName") is not None:
        query_params["LastName"] = getattr(input, "LastName")
    if getattr(input, "PageNumber") is not None:
        query_params["PageNumber"] = getattr(input, "PageNumber")
    if getattr(input, "PageSize") is not None:
        query_params["PageSize"] = getattr(input, "PageSize")
    if getattr(input, "Sort") is not None:
        query_params["Sort"] = getattr(input, "Sort")
    try:
        from pydantic import ValidationError
        with tracer.start_as_current_span("tool_call") as span:
            span.set_attribute("tool.name", "get_api_v1_customers")
            res = await call_api("GET", f"/api/v1/customers", params=query_params, tool="get_api_v1_customers")
            tool_invocations_total.labels(tool="get_api_v1_customers", outcome="success").inc()
            return create_output(get_api_v1_customers_Output, res)
    except ValidationError:
        tool_validation_errors_total.labels(tool="get_api_v1_customers").inc()
        return get_api_v1_customers_Output()
    except Exception:
        tool_invocations_total.labels(tool="get_api_v1_customers", outcome="error").inc()
        return get_api_v1_customers_Output()
