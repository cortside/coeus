from opentelemetry.trace import Status, StatusCode
from utils.tracing import get_tracer
from app.mcp_instance import mcp
from utils.client import call_api
from utils.metrics import tool_invocations_total, tool_validation_errors_total
from .get_api_v1_customers_search_models import get_api_v1_customers_search_Input
from schemas.models import Cortside_AspNetCore_Common_Paging_PagedListOf_Acme_ShoppingCart_WebApi_Models_Responses_CustomerModel

tracer = get_tracer("acme.shoppingcart.mcp.tool.get_api_v1_customers_search")


@mcp.tool(name="get_api_v1_customers_search", description="Returns search results for rebate requests (Auth permission: GetCustomers)", tags={"route:/api/v1/customers/search", "method:GET"})
async def get_api_v1_customers_search(input: get_api_v1_customers_search_Input) -> Cortside_AspNetCore_Common_Paging_PagedListOf_Acme_ShoppingCart_WebApi_Models_Responses_CustomerModel:
    tool_invocations_total.labels(tool="get_api_v1_customers_search", outcome="started").inc()
    query_params = {}
    if getattr(input, "encryptedParams") is not None:
        query_params["encryptedParams"] = getattr(input, "encryptedParams")
    try:
        from pydantic import ValidationError
        with tracer.start_as_current_span("tool_call") as span:
            span.set_attribute("tool.name", "get_api_v1_customers_search")
            res = await call_api("GET", "/api/v1/customers/search", params=query_params, tool="get_api_v1_customers_search")
            tool_invocations_total.labels(tool="get_api_v1_customers_search", outcome="success").inc()
            return Cortside_AspNetCore_Common_Paging_PagedListOf_Acme_ShoppingCart_WebApi_Models_Responses_CustomerModel.parse_obj(res)
    except ValidationError:
        tool_validation_errors_total.labels(tool="get_api_v1_customers_search").inc()
        return Cortside_AspNetCore_Common_Paging_PagedListOf_Acme_ShoppingCart_WebApi_Models_Responses_CustomerModel(
            totalItems=None,
            pageNumber=None,
            pageSize=None,
            totalPages=None,
            items=None
        )
    except Exception:
        tool_invocations_total.labels(tool="get_api_v1_customers_search", outcome="error").inc()
        return Cortside_AspNetCore_Common_Paging_PagedListOf_Acme_ShoppingCart_WebApi_Models_Responses_CustomerModel(
            totalItems=None,
            pageNumber=None,
            pageSize=None,
            totalPages=None,
            items=None
        )
