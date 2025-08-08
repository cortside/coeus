from fastapi import FastAPI, Request
from fastapi.responses import JSONResponse
def add_error_handlers(app: FastAPI):
    @app.exception_handler(Exception)
    async def on_exception(request: Request, exc: Exception):
        return JSONResponse(status_code=500, content={"error": str(exc)})
