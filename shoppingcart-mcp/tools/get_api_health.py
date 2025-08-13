from opentelemetry.trace import Status, StatusCode
from utils.tracing import get_tracer
from app.mcp_instance import mcp
from utils.client import call_api
from utils.metrics import tool_invocations_total, tool_validation_errors_total
from .get_api_health_models import get_api_health_Input, get_api_health_Output
from schemas.models import Cortside_Health_Models_HealthModel
from utils.output_helper import create_output
tracer = get_tracer("acme.shoppingcart.mcp.tool.get_api_health")
@mcp.tool(name="get_api_health", description="get_api_health", tags={"route:/api/health", "method:GET"})
async def get_api_health(input: get_api_health_Input) -> Cortside_Health_Models_HealthModel:
    tool_invocations_total.labels(tool="get_api_health", outcome="started").inc()
    query_params = {}
    try:
        from pydantic import ValidationError
        with tracer.start_as_current_span("tool_call") as span:
            span.set_attribute("tool.name", "get_api_health")
            res = await call_api("GET", f"/api/health", params=query_params, tool="get_api_health")
            health = Cortside_Health_Models_HealthModel.parse_obj(res)
            tool_invocations_total.labels(tool="get_api_health", outcome="success").inc()
            return health
    except ValidationError:
        tool_validation_errors_total.labels(tool="get_api_health").inc()
        return Cortside_Health_Models_HealthModel()
    except Exception:
        tool_invocations_total.labels(tool="get_api_health", outcome="error").inc()
        return Cortside_Health_Models_HealthModel(
            service=None,
            build=None,
            checks=None,
            uptime=None,
            healthy=None,
            status=None,
            statusDetail=None,
            timestamp=None,
            required=None,
            availability=None
        )
