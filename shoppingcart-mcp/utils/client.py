import asyncio, httpx, logging, os, time
from typing import Any, Dict, Optional, Union
from utils.oauth import OAuth2Client
from utils.circuit_breaker import registry, CircuitOpenError
from utils.metrics import http_requests_total, http_request_latency_seconds, set_cb_state
from utils.tracing import get_tracer

API_BASE="https://shoppingcartapi.cortside.net/api"
RETRIES=int(os.getenv("SHOPPINGCART_RETRIES","3"))
TIMEOUT_SECONDS=float(os.getenv("SHOPPINGCART_TIMEOUT_SECONDS","15"))

oauth_client=OAuth2Client()
logger=logging.getLogger("mcp_server.client")
tracer=get_tracer("acme.shoppingcart.mcp.client")

async def call_api(method:str, path:str, params:Optional[Dict[str,Any]]=None, json:Optional[Any]=None, tool:Optional[str]=None)->Union[Dict[str,Any],str]:
    token=await oauth_client.get_token()
    url=f"{API_BASE}{path}"
    headers={ "Authorization": f"Bearer {token}", "Accept": "application/json" }
    cb=registry.get(method, path)
    tool=tool or "unknown"; attempt=0
    timeout=httpx.Timeout(TIMEOUT_SECONDS)
    async with httpx.AsyncClient(timeout=timeout) as client:
        while attempt<max(1,RETRIES):
            start=time.monotonic()
            try:
                cb.allow_request(); set_cb_state(tool,method,path,cb.state)
                with tracer.start_as_current_span("http_call") as span:
                    span.set_attribute("tool", str(tool)); span.set_attribute("http.method", method); span.set_attribute("http.url", url)
                    resp = await client.request(method, url, params=params, json=json, headers=headers)
                    try:
                        data = resp.json()
                    except Exception:
                        data = {"raw": resp.text}
                    latency=time.monotonic()-start
                    http_request_latency_seconds.labels(tool=tool,method=method,path=path).observe(latency)
                    if resp.status_code>=500:
                        http_requests_total.labels(tool=tool,method=method,path=path,outcome="server_error").inc()
                        logger.warning("Server error %s on %s %s: %s", resp.status_code, method, url, data)
                        cb.record_failure(); set_cb_state(tool,method,path,cb.state); span.set_attribute("http.status_code",resp.status_code)
                        raise httpx.HTTPStatusError(f"Server error {resp.status_code}", request=resp.request, response=resp)
                    if resp.status_code>=400:
                        http_requests_total.labels(tool=tool,method=method,path=path,outcome="client_error").inc()
                        cb.record_success(); set_cb_state(tool,method,path,cb.state); span.set_attribute("http.status_code",resp.status_code)
                        return {"status": resp.status_code, "error": data}
                    http_requests_total.labels(tool=tool,method=method,path=path,outcome="success").inc()
                    cb.record_success(); set_cb_state(tool,method,path,cb.state); span.set_attribute("http.status_code", resp.status_code)
                    return data
            except CircuitOpenError as e:
                latency=time.monotonic()-start; http_request_latency_seconds.labels(tool=tool,method=method,path=path).observe(latency)
                http_requests_total.labels(tool=tool,method=method,path=path,outcome="circuit_open").inc()
                logger.error("Circuit open for %s %s: %s", method, path, str(e))
                set_cb_state(tool,method,path,"open")
                return {"status":0,"error":{"message":"circuit_open"}}
            except (httpx.HTTPError, asyncio.TimeoutError) as e:
                latency=time.monotonic()-start; http_request_latency_seconds.labels(tool=tool,method=method,path=path).observe(latency)
                http_requests_total.labels(tool=tool,method=method,path=path,outcome="timeout" if isinstance(e, asyncio.TimeoutError) else "exception").inc()
                attempt+=1; logger.warning("Transport/timeout error on %s %s: %s (attempt %d)", method, url, repr(e), attempt)
                cb.record_failure(); set_cb_state(tool,method,path,cb.state)
                if attempt>=max(1,RETRIES):
                    logger.error("HTTP request failed after %d attempts: %s %s (%s)", attempt, method, url, repr(e))
                    return {"status":0,"error":{"message":"request_failed","detail":str(e)}}
                await asyncio.sleep(min(2**attempt,10)+(attempt*0.05))
