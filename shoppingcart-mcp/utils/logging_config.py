import logging, os
from utils.tracing import get_logging_handler
def configure_logging():
    level = getattr(logging, os.getenv("LOG_LEVEL","INFO").upper(), logging.INFO)
    logging.basicConfig(level=level, format="%(asctime)s %(levelname)s [%(name)s] %(message)s")
    try: logging.getLogger().addHandler(get_logging_handler())
    except Exception: pass
    return logging.getLogger("mcp_server")
