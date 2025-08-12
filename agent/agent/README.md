# agent-backend

This is the backend service for the chat application, built with FastAPI. It connects to the MCP server, provides authentication via OpenID Connect, and exposes chat, health, and metrics endpoints.

## Features
- FastAPI async backend
- OpenID Connect (OIDC) authentication
- MCP tool HTTP integration with retries
- In-memory per-user chat history
- Prometheus metrics and OpenTelemetry tracing
- Endpoints: `/chat` (POST, streaming SSE/WebSocket), `/health`, `/metrics`

## Requirements
- Python 3.11+
- (Recommended) Virtual environment

## Setup

1. Install dependencies:
   ```powershell
   python -m venv .venv
   .venv\Scripts\Activate.ps1
   pip install -r requirements.txt
   ```
2. Copy `.env.example` to `.env` and adjust as needed.

## Running the Service

- To start the backend:
  ```powershell
  uvicorn main:app --host 0.0.0.0 --port 8000
  ```
- Or use the provided script:
  ```powershell
  ./run.ps1
  ```

## Development
- Linting and formatting are recommended (e.g., with `black`, `flake8`).
- Prometheus metrics available at `/metrics`.
- Health check at `/health`.

## License
MIT
