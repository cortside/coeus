from opentelemetry.trace import Status, StatusCode
from utils.tracing import get_tracer
from app.mcp_instance import mcp
from utils.client import call_api
from utils.metrics import tool_invocations_total, tool_validation_errors_total
from .get_api_v1_customers_models import get_api_v1_customers_Input
from schemas.models import Cortside_AspNetCore_Common_Paging_PagedListOf_Acme_ShoppingCart_WebApi_Models_Responses_CustomerModel
tracer = get_tracer("acme.shoppingcart.mcp.tool.get_api_v1_customers")
@mcp.tool(name="get_api_v1_customers", description="Gets customers (Auth permission: GetCustomers)", tags={"route:/api/v1/customers", "method:GET"})
async def get_api_v1_customers(input: get_api_v1_customers_Input) -> Cortside_AspNetCore_Common_Paging_PagedListOf_Acme_ShoppingCart_WebApi_Models_Responses_CustomerModel:
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
            res = await call_api("GET", "/api/v1/customers", params=query_params, tool="get_api_v1_customers")
            tool_invocations_total.labels(tool="get_api_v1_customers", outcome="success").inc()
            return Cortside_AspNetCore_Common_Paging_PagedListOf_Acme_ShoppingCart_WebApi_Models_Responses_CustomerModel.parse_obj(res)
    except ValidationError:
        tool_validation_errors_total.labels(tool="get_api_v1_customers").inc()
        return Cortside_AspNetCore_Common_Paging_PagedListOf_Acme_ShoppingCart_WebApi_Models_Responses_CustomerModel(
            totalItems=None,
            pageNumber=None,
            pageSize=None,
            totalPages=None,
            items=None
        )
    except Exception:
        tool_invocations_total.labels(tool="get_api_v1_customers", outcome="error").inc()
        return Cortside_AspNetCore_Common_Paging_PagedListOf_Acme_ShoppingCart_WebApi_Models_Responses_CustomerModel(
            totalItems=None,
            pageNumber=None,
            pageSize=None,
            totalPages=None,
            items=None
        )
