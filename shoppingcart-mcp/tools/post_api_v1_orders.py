from opentelemetry.trace import Status, StatusCode
from utils.tracing import get_tracer
from app.mcp_instance import mcp
from utils.client import call_api
from utils.metrics import tool_invocations_total, tool_validation_errors_total
from .post_api_v1_orders_models import post_api_v1_orders_Input
from schemas.models import Acme_ShoppingCart_WebApi_Models_Responses_OrderModel
tracer = get_tracer("acme.shoppingcart.mcp.tool.post_api_v1_orders")
@mcp.tool(name="post_api_v1_orders", description="Create an order (Auth permission: CreateOrder)", tags={"route:/api/v1/orders", "method:POST"})
async def post_api_v1_orders(input: post_api_v1_orders_Input) -> Acme_ShoppingCart_WebApi_Models_Responses_OrderModel:
    tool_invocations_total.labels(tool="post_api_v1_orders", outcome="started").inc()
    query_params = {}
    try:
        from pydantic import ValidationError
        with tracer.start_as_current_span("tool_call") as span:
            span.set_attribute("tool.name", "post_api_v1_orders")
            res = await call_api("POST", "/api/v1/orders", params=query_params, json=input.body, tool="post_api_v1_orders")
            tool_invocations_total.labels(tool="post_api_v1_orders", outcome="success").inc()
            return Acme_ShoppingCart_WebApi_Models_Responses_OrderModel.parse_obj(res)
    except ValidationError:
        tool_validation_errors_total.labels(tool="post_api_v1_orders").inc()
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
        tool_invocations_total.labels(tool="post_api_v1_orders", outcome="error").inc()
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
