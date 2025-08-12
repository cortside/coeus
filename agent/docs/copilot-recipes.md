# Copilot Recipes – Python MCP Agent + React Chat UI

## 1. Create a new backend endpoint
**Prompt:**
"Add an async FastAPI GET endpoint `/my-endpoint` in `agent` that requires JWT auth, logs with OTEL, increments a Prometheus counter, and returns `{ 'status': 'ok' }`."

## 2. Add a new MCP tool binding
**Prompt:**
"Implement a function in `mcp_client.py` called `call_tool_x` that POSTs to MCP server over HTTP, validates response JSON schema, retries up to 3 times, and returns parsed output."

## 3. Implement the chat POST endpoint
**Prompt:**
"Create `/chat` endpoint in FastAPI that:
- Authenticates user
- Appends user message to in-memory chat history
- Sends messages + history to GPT-4.1
- Invokes MCP tools if model requests
- Streams assistant response back to client
- Records metrics (tokens, latency) and OTEL spans"

## 4. Add OpenID Connect auth to backend
**Prompt:**
"Integrate FastAPI with OpenID Connect provider https://identityserver.cortside.net:
- Use JWT bearer middleware
- Validate RS256 token signature against provider's JWKS
- Require valid access token for all endpoints except `/health` and `/metrics`."

## 5. Create React chat UI page
**Prompt:**
"Create a `ChatPage.tsx` that:
- Uses OpenID Connect login to get JWT
- Displays messages in scrollable chat list
- Has input box with send button
- Calls `/chat` via WebSocket for streaming
- Renders markdown messages from assistant"

## 6. Add Prometheus + OTEL
**Prompt:**
"Set up Prometheus metrics in FastAPI using `prometheus_client` and OTEL tracing using `opentelemetry-instrumentation-fastapi`. Add `/metrics` endpoint and instrument `/chat` and MCP tool calls."

## 7. Docker Compose setup
**Prompt:**
"Create `docker-compose.yml` that runs:
- Backend service
- Frontend service
- Prometheus
- Grafana with preloaded dashboard JSON"

## 8. K8s manifests
**Prompt:**
"Create manifests in `k8s/` for backend and frontend deployments, services, ingress, Prometheus, Grafana."
