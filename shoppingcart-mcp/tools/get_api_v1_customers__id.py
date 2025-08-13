from opentelemetry.trace import Status, StatusCode
from utils.tracing import get_tracer
from app.mcp_instance import mcp
from utils.client import call_api
from utils.metrics import tool_invocations_total, tool_validation_errors_total
from .get_api_v1_customers__id_models import get_api_v1_customers__id_Input
from schemas.models import Acme_ShoppingCart_WebApi_Models_Responses_CustomerModel
tracer = get_tracer("acme.shoppingcart.mcp.tool.get_api_v1_customers__id")
@mcp.tool(name="get_api_v1_customers__id", description="Gets a customer by id (Auth permission: GetCustomer)", tags={"route:/api/v1/customers/{id}", "method:GET"})
async def get_api_v1_customers__id(input: get_api_v1_customers__id_Input) -> Acme_ShoppingCart_WebApi_Models_Responses_CustomerModel:
    tool_invocations_total.labels(tool="get_api_v1_customers__id", outcome="started").inc()
    query_params = {}
    try:
        from pydantic import ValidationError
        with tracer.start_as_current_span("tool_call") as span:
            span.set_attribute("tool.name", "get_api_v1_customers__id")
            res = await call_api("GET", f"/api/v1/customers/{input.id}", params=query_params, tool="get_api_v1_customers__id")
            tool_invocations_total.labels(tool="get_api_v1_customers__id", outcome="success").inc()
            return Acme_ShoppingCart_WebApi_Models_Responses_CustomerModel.parse_obj(res)
    except ValidationError:
        tool_validation_errors_total.labels(tool="get_api_v1_customers__id").inc()
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
        tool_invocations_total.labels(tool="get_api_v1_customers__id", outcome="error").inc()
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
