from fastapi import FastAPI
from app.mcp_instance import mcp
import tools  # noqa: F401 - registers tools via decorators
from utils.error_handler import add_error_handlers
from utils.logging_config import configure_logging
from utils.tracing import setup_tracing
from prometheus_client import make_asgi_app

logger = configure_logging()

# Create ASGI app from MCP and mount it
mcp_app = mcp.http_app(path="/mcp")

# FastAPI app must use the MCP lifespan
app = FastAPI(title="Acme.ShoppingCart MCP (FastMCP v2)", lifespan=mcp_app.lifespan)

# Mount MCP under /mcp (HTTP endpoint for MCP)
app.mount("/mcp", mcp_app)

# Public /metrics endpoint
metrics_app = make_asgi_app()
app.mount("/metrics", metrics_app)

setup_tracing(app)
add_error_handlers(app)

@app.get("/")
async def root():
    return {
        "message": "Acme.ShoppingCart MCP Server (v2) is running",
        "mcp_path": "/mcp/",
        "tools": [t.name for t in mcp.tools]
    }

@app.get("/live")
async def live():
    return {"status": "ok"}

@app.get("/ready")
async def ready():
    try:
        from utils.oauth import OAuth2Client
        _ = await OAuth2Client().get_token()
        return {"status": "ready"}
    except Exception as e:
        return {"status": "not_ready", "error": str(e)}
