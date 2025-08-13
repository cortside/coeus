from opentelemetry.trace import Status, StatusCode
from utils.tracing import get_tracer
from app.mcp_instance import mcp
from utils.client import call_api
from utils.metrics import tool_invocations_total, tool_validation_errors_total
from .put_api_v1_customers__id_models import put_api_v1_customers__id_Input
from schemas.models import Acme_ShoppingCart_WebApi_Models_Responses_CustomerModel
tracer = get_tracer("acme.shoppingcart.mcp.tool.put_api_v1_customers__id")
@mcp.tool(name="put_api_v1_customers__id", description="Update a customer (Auth permission: UpdateCustomer)", tags={"route:/api/v1/customers/{id}", "method:PUT"})
async def put_api_v1_customers__id(input: put_api_v1_customers__id_Input) -> Acme_ShoppingCart_WebApi_Models_Responses_CustomerModel:
    tool_invocations_total.labels(tool="put_api_v1_customers__id", outcome="started").inc()
    query_params = {}
    try:
        from pydantic import ValidationError
        with tracer.start_as_current_span("tool_call") as span:
            span.set_attribute("tool.name", "put_api_v1_customers__id")
            res = await call_api("PUT", f"/api/v1/customers/{input.id}", params=query_params, json=input.body, tool="put_api_v1_customers__id")
            tool_invocations_total.labels(tool="put_api_v1_customers__id", outcome="success").inc()
            return Acme_ShoppingCart_WebApi_Models_Responses_CustomerModel.parse_obj(res)
    except ValidationError:
        tool_validation_errors_total.labels(tool="put_api_v1_customers__id").inc()
        return Acme_ShoppingCart_WebApi_Models_Responses_CustomerModel()
    except Exception:
        tool_invocations_total.labels(tool="put_api_v1_customers__id", outcome="error").inc()
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
