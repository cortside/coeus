from opentelemetry.trace import Status, StatusCode
from utils.tracing import get_tracer
from app.mcp_instance import mcp
from utils.client import call_api
from utils.metrics import tool_invocations_total, tool_validation_errors_total
from .get_api_v1_authorization_models import get_api_v1_authorization_Input
from schemas.models import Acme_ShoppingCart_WebApi_Models_Responses_AuthorizationModel
tracer = get_tracer("acme.shoppingcart.mcp.tool.get_api_v1_authorization")
@mcp.tool(name="get_api_v1_authorization", description="Gets the list if permissions associated with the caller, determined by their bearer token (Auth)", tags={"route:/api/v1/authorization", "method:GET"})
async def get_api_v1_authorization(input: get_api_v1_authorization_Input) -> Acme_ShoppingCart_WebApi_Models_Responses_AuthorizationModel:
    tool_invocations_total.labels(tool="get_api_v1_authorization", outcome="started").inc()
    query_params = {}
    try:
        from pydantic import ValidationError
        with tracer.start_as_current_span("tool_call") as span:
            span.set_attribute("tool.name", "get_api_v1_authorization")
            res = await call_api("GET", "/api/v1/authorization", params=query_params, tool="get_api_v1_authorization")
            tool_invocations_total.labels(tool="get_api_v1_authorization", outcome="success").inc()
            return Acme_ShoppingCart_WebApi_Models_Responses_AuthorizationModel.parse_obj(res)
    except ValidationError:
        tool_validation_errors_total.labels(tool="get_api_v1_authorization").inc()
        return Acme_ShoppingCart_WebApi_Models_Responses_AuthorizationModel(
            roles=None,
            permissions=None,
            principal=None
        )
    except Exception:
        tool_invocations_total.labels(tool="get_api_v1_authorization", outcome="error").inc()
        return Acme_ShoppingCart_WebApi_Models_Responses_AuthorizationModel(
            roles=None,
            permissions=None,
            principal=None
        )
