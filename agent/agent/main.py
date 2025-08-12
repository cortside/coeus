import os
from fastapi import FastAPI, Depends, WebSocket, Request, status
from fastapi.responses import StreamingResponse, JSONResponse
from fastapi.middleware.cors import CORSMiddleware
from prometheus_fastapi_instrumentator import Instrumentator
from opentelemetry.instrumentation.fastapi import FastAPIInstrumentor
from .auth import get_current_user, AuthError
from .mcp_client import MCPClient
from .chat_history import ChatHistory
import asyncio

app = FastAPI()

# CORS
app.add_middleware(
    CORSMiddleware,
    allow_origins=["*"],
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
)

# Observability
Instrumentator().instrument(app).expose(app, include_in_schema=False, should_gzip=True)
FastAPIInstrumentor.instrument_app(app)

mcp_client = MCPClient()
chat_history = ChatHistory()

@app.post("/chat")
async def chat(request: Request, user=Depends(get_current_user)):
    data = await request.json()
    user_id = user["sub"]
    prompt = data.get("prompt")
    if not prompt:
        return JSONResponse({"error": "Prompt required"}, status_code=400)
    async def event_stream():
        async for chunk in mcp_client.stream_chat(prompt, user_id):
            yield chunk
    chat_history.add_message(user_id, prompt)
    return StreamingResponse(event_stream(), media_type="text/event-stream")

@app.websocket("/chat")
async def chat_ws(websocket: WebSocket, user=Depends(get_current_user)):
    await websocket.accept()
    try:
        while True:
            data = await websocket.receive_json()
            prompt = data.get("prompt")
            user_id = user["sub"]
            chat_history.add_message(user_id, prompt)
            async for chunk in mcp_client.stream_chat(prompt, user_id):
                await websocket.send_text(chunk)
    except Exception:
        await websocket.close()

@app.get("/health")
async def health():
    return {"status": "ok"}

@app.get("/metrics")
async def metrics():
    # Prometheus metrics auto-exposed
    pass
