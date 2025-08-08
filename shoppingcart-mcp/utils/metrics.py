from prometheus_client import Counter, Gauge, Histogram
http_requests_total = Counter("mcp_http_requests_total","Total HTTP requests made by the MCP client",labelnames=("tool","method","path","outcome"))
http_request_latency_seconds = Histogram("mcp_http_request_latency_seconds","HTTP request latency in seconds",labelnames=("tool","method","path"),buckets=(0.05,0.1,0.25,0.5,1,2,5,10,float("inf")))
circuit_breaker_state = Gauge("mcp_circuit_breaker_state","Circuit breaker state per route (0=closed,1=open,2=half_open)",labelnames=("tool","method","path"))
def set_cb_state(tool:str, method:str, path:str, state:str): circuit_breaker_state.labels(tool=tool,method=method,path=path).set({"closed":0,"open":1,"half_open":2}.get(state,0))
tool_invocations_total = Counter("mcp_tool_invocations_total","Total MCP tool invocations",labelnames=("tool","outcome"))
tool_validation_errors_total = Counter("mcp_tool_validation_errors_total","Total MCP tool validation errors",labelnames=("tool",))
