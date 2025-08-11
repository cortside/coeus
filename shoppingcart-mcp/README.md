# Acme ShoppingCart MCP Server — FastMCP v2 (full stack)

- **FastMCP v2** (`from fastmcp import FastMCP`) with `@mcp.tool` per OpenAPI route
- Async HTTPX client + OAuth2 (client credentials), retries, exponential backoff, circuit-breaker
- Prometheus `/metrics` (public), per-route labels & tool counters
- OpenTelemetry (HTTPX + FastAPI), OTLP/HTTP exporters
- Dockerfile + docker-compose (OTel Collector, Prometheus, Grafana)
- **Full** Kubernetes manifests (app + OTel Collector + Prometheus + Grafana), with ConfigMaps
- Grafana dashboard (prometheus datasource + pre-provisioned panels)

## Run locally

```bash
pip install -r requirements.txt
uvicorn main:app --host 0.0.0.0 --port 8000
# MCP:      http://localhost:8000/mcp/
# Metrics:  http://localhost:8000/metrics
```

## Docker Compose

```bash
docker compose up --build
# Grafana http://localhost:3000  (admin/admin)
```

## Kubernetes quickstart

```bash
kubectl apply -k k8s/
```

## Env
See `.env.example` for required settings (SHOPPINGCART_CLIENT_ID/SECRET).


## MCP Inspector
Use the following to test mcp server directly

```
npx @modelcontextprotocol/inspector
```

Transport type = streamable http
url = http://localhost:8080/mcp (change hostname and port for how it's hosted)
