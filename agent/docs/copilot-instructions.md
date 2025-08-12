# Copilot Instructions – Python MCP Agent + React Chat UI

You are working in a 2-repo project:
- `agent` – Python 3.11 FastAPI agent that connects to an MCP server over HTTP
- `chatbot` – React 19 + TypeScript chat UI

## Architecture
- Backend: FastAPI, async/await, connects to MCP server over HTTP
- LLM: gpt-4.1, streaming responses
- Auth: OpenID Connect via https://identityserver.cortside.net, JWT validation middleware
- Chat history: in-memory per-user (dict keyed by user_id)
- Observability: Prometheus `/metrics`, OpenTelemetry tracing, Grafana dashboards
- Infra: Docker Compose + Kubernetes manifests, `.env` for config
- License: MIT

## Backend Conventions
- All endpoints async
- `/chat` (POST, streaming SSE/WebSocket)
- `/health` (liveness/readiness checks)
- `/metrics` (Prometheus format)
- JWT middleware validates access token and extracts `sub` as `user_id`
- MCP HTTP client handles tool invocation, retries, and error handling
- Instrument with OTEL spans and metrics counters per route/tool

## Frontend Conventions
- Vite + React 19 + TypeScript + Tailwind + shadcn/ui
- Auth via OpenID Connect (PKCE) → store JWT in memory
- Chat page with:
  - message list
  - input box
  - streaming updates
  - multi-conversation sidebar (future)
- Call backend `/chat` via fetch or WebSocket for streaming

## Infra Conventions
- Dockerfile per repo
- `docker-compose.yml` to run backend, frontend, Prometheus, Grafana
- `k8s/` with manifests for backend, frontend, Prometheus, Grafana
- `.env` loaded with `python-dotenv` for backend, Vite env vars for frontend
- Metrics labeled with `service`, `route`, `status_code`

## Development Flow
1. When asked to add a backend feature:
   - Add route in FastAPI
   - Add OTEL instrumentation
   - Add Prometheus metrics counter
   - Update docs if new tool or endpoint
2. When asked to add a frontend feature:
   - Use functional components with hooks
   - Strongly type props and state
   - Use Tailwind + shadcn/ui
   - Make calls to backend through API client
3. Keep tests: `pytest` for backend, `vitest` for frontend
4. Always run `docker-compose up` locally to validate full stack
