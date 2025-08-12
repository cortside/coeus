import os
from fastapi import Request, HTTPException, status, Depends
from jose import jwt, JWTError
import httpx

OIDC_ISSUER = os.getenv("OIDC_ISSUER", "https://identityserver.cortside.net")
OIDC_AUDIENCE = os.getenv("OIDC_AUDIENCE", "agent-backend")
OIDC_JWKS_URL = f"{OIDC_ISSUER}/.well-known/openid-configuration/jwks"

class AuthError(Exception):
    pass

async def get_jwks():
    async with httpx.AsyncClient() as client:
        resp = await client.get(OIDC_JWKS_URL)
        return resp.json()["keys"]

async def get_current_user(request: Request):
    auth = request.headers.get("Authorization")
    if not auth or not auth.startswith("Bearer "):
        raise HTTPException(status_code=status.HTTP_401_UNAUTHORIZED, detail="Missing token")
    token = auth.split(" ", 1)[1]
    try:
        # For demo: skip key rotation, use first key
        jwks = await get_jwks()
        key = jwks[0]
        payload = jwt.decode(token, key, audience=OIDC_AUDIENCE, issuer=OIDC_ISSUER, algorithms=["RS256"])
        return payload
    except JWTError:
        raise HTTPException(status_code=status.HTTP_401_UNAUTHORIZED, detail="Invalid token")
