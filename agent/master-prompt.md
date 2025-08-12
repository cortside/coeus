You are a senior software engineer generating a complete application with two repos:  
`agent-backend` (FastAPI MCP agent) and `chat-frontend` (React chat UI).

### Requirements
- Backend: Python 3.11, FastAPI, async/await, connects to MCP server over HTTP, LLM is GPT-4.1, streaming responses
- Auth: OpenID Connect via https://identityserver.cortside.net, JWT validation middleware
- Chat history: in-memory per-user
- Observability: Prometheus `/metrics`, OpenTelemetry tracing, Grafana dashboards
- Endpoints: `/chat` (POST, streaming SSE/WebSocket), `/health`, `/metrics`
- Infra: Docker Compose + Kubernetes manifests, `.env` for config
- License: MIT
- Output in 2 repos: `agent-backend` and `chat-frontend`
- Frontend: Vite + React 19 + TypeScript + Tailwind + shadcn/ui
- Frontend Auth: OIDC login (PKCE), store JWT in memory
- Docker + k8s manifests for backend, frontend, Prometheus, Grafana

### Deliverables
1. **agent-backend**:
   - `main.py` FastAPI app
   - Auth middleware for JWT validation
   - MCP client HTTP integration with retries
   - `/chat`, `/health`, `/metrics` endpoints
   - Prometheus metrics and OTEL tracing
   - Dockerfile, requirements.txt, `.env.example`
   - k8s manifests

2. **chat-frontend**:
   - Vite + React 19 + TypeScript + Tailwind + shadcn/ui
   - Login via OIDC PKCE
   - Chat page with streaming messages via WebSocket
   - API client for backend
   - Dockerfile, `.env.example`
   - k8s manifests

3. **Infra**:
   - `docker-compose.yml` to run backend, frontend, Prometheus, Grafana
   - Grafana dashboard JSON for chat metrics

Generate all code in directory structures for both repos.
