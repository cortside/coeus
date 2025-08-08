import os, time, httpx
from typing import Optional
TOKEN_URL = "https://identityserver.cortside.net/connect/token"
SCOPE = os.getenv("SHOPPINGCART_SCOPE", "shoppingcart-api")
CLIENT_ID = os.getenv("SHOPPINGCART_CLIENT_ID", "")
CLIENT_SECRET = os.getenv("SHOPPINGCART_CLIENT_SECRET", "")
class OAuth2Client:
    def __init__(self, token_url: str = TOKEN_URL, client_id: str = CLIENT_ID, client_secret: str = CLIENT_SECRET, scope: str = SCOPE):
        self.token_url = token_url; self.client_id = client_id; self.client_secret = client_secret; self.scope = scope
        self._token: Optional[str] = None; self._expiry: float = 0.0
    async def get_token(self) -> str:
        if self._token and self._expiry > time.time(): return self._token
        data = {"grant_type":"client_credentials","client_id":self.client_id,"client_secret":self.client_secret,"scope":self.scope}
        async with httpx.AsyncClient(timeout=20.0) as client:
            resp = await client.post(self.token_url, data=data)
            resp.raise_for_status()
            payload = resp.json(); self._token = payload["access_token"]; self._expiry = time.time()+payload.get("expires_in",3600)-60; return self._token
