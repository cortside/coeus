from pydantic import BaseModel, Field
from typing import Optional
from schemas.models import *

class get_api_v1_customers_search_Input(BaseModel):
    encryptedParams: Optional[str] = None