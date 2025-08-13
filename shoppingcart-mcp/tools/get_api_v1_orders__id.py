from opentelemetry.trace import Status, StatusCode
from utils.tracing import get_tracer
from app.mcp_instance import mcp
from utils.client import call_api
from utils.metrics import tool_invocations_total, tool_validation_errors_total
from .get_api_v1_orders__id_models import get_api_v1_orders__id_Input
from schemas.models import Acme_ShoppingCart_WebApi_Models_Responses_OrderModel
tracer = get_tracer("acme.shoppingcart.mcp.tool.get_api_v1_orders__id")
@mcp.tool(name="get_api_v1_orders__id", description="Get order by id (Auth permission: GetOrders)", tags={"route:/api/v1/orders/{id}", "method:GET"})
async def get_api_v1_orders__id(input: get_api_v1_orders__id_Input) -> Acme_ShoppingCart_WebApi_Models_Responses_OrderModel:
    tool_invocations_total.labels(tool="get_api_v1_orders__id", outcome="started").inc()
    query_params = {}
    try:
        from pydantic import ValidationError
        with tracer.start_as_current_span("tool_call") as span:
            span.set_attribute("tool.name", "get_api_v1_orders__id")
            res = await call_api("GET", f"/api/v1/orders/{input.id}", params=query_params, tool="get_api_v1_orders__id")
            tool_invocations_total.labels(tool="get_api_v1_orders__id", outcome="success").inc()
            return Acme_ShoppingCart_WebApi_Models_Responses_OrderModel.parse_obj(res)
    except ValidationError:
        tool_validation_errors_total.labels(tool="get_api_v1_orders__id").inc()
        return Acme_ShoppingCart_WebApi_Models_Responses_OrderModel(
            orderResourceId=None,
            status=None,
            customer=None,
            address=None,
            items=None,
            createdDate=None,
            createdSubject=None,
            lastModifiedDate=None,
            lastModifiedSubject=None
        )
    except Exception:
        tool_invocations_total.labels(tool="get_api_v1_orders__id", outcome="error").inc()
        return Acme_ShoppingCart_WebApi_Models_Responses_OrderModel(
            orderResourceId=None,
            status=None,
            customer=None,
            address=None,
            items=None,
            createdDate=None,
            createdSubject=None,
            lastModifiedDate=None,
            lastModifiedSubject=None
        )
