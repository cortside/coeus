from opentelemetry.trace import Status, StatusCode
from utils.tracing import get_tracer
from app.mcp_instance import mcp
from utils.client import call_api
from utils.metrics import tool_invocations_total, tool_validation_errors_total
from .post_api_v1_customers_models import post_api_v1_customers_Input
from schemas.models import Acme_ShoppingCart_WebApi_Models_Responses_CustomerModel
tracer = get_tracer("acme.shoppingcart.mcp.tool.post_api_v1_customers")
@mcp.tool(name="post_api_v1_customers", description="Create a new customer (Auth permission: CreateCustomer)", tags={"route:/api/v1/customers", "method:POST"})
async def post_api_v1_customers(input: post_api_v1_customers_Input) -> Acme_ShoppingCart_WebApi_Models_Responses_CustomerModel:
    tool_invocations_total.labels(tool="post_api_v1_customers", outcome="started").inc()
    query_params = {}
    try:
        from pydantic import ValidationError
        with tracer.start_as_current_span("tool_call") as span:
            span.set_attribute("tool.name", "post_api_v1_customers")
            res = await call_api("POST", "/api/v1/customers", params=query_params, json=input.body, tool="post_api_v1_customers")
            tool_invocations_total.labels(tool="post_api_v1_customers", outcome="success").inc()
            return Acme_ShoppingCart_WebApi_Models_Responses_CustomerModel.parse_obj(res)
    except ValidationError:
        tool_validation_errors_total.labels(tool="post_api_v1_customers").inc()
        return Acme_ShoppingCart_WebApi_Models_Responses_CustomerModel(
            customerResourceId=None,
            firstName=None,
            lastName=None,
            email=None,
            createdDate=None,
            createdSubject=None,
            lastModifiedDate=None,
            lastModifiedSubject=None
        )
    except Exception:
        tool_invocations_total.labels(tool="post_api_v1_customers", outcome="error").inc()
        return Acme_ShoppingCart_WebApi_Models_Responses_CustomerModel(
            customerResourceId=None,
            firstName=None,
            lastName=None,
            email=None,
            createdDate=None,
            createdSubject=None,
            lastModifiedDate=None,
            lastModifiedSubject=None
        )
