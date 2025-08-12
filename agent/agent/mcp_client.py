import os
import httpx
import asyncio

MCP_SERVER_URL = os.getenv("MCP_SERVER_URL", "http://mcp-server:8000")

class MCPClient:
    def __init__(self):
        self.base_url = MCP_SERVER_URL
        self.client = httpx.AsyncClient()

    async def stream_chat(self, prompt: str, user_id: str):
        url = f"{self.base_url}/chat"
        retries = 3
        for attempt in range(retries):
            try:
                async with self.client.stream("POST", url, json={"prompt": prompt, "user_id": user_id}) as resp:
                    async for line in resp.aiter_lines():
                        yield line
                break
            except Exception as e:
                if attempt == retries - 1:
                    raise e
                await asyncio.sleep(1)
