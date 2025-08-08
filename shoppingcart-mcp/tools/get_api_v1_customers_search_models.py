from pydantic import BaseModel, Field
from typing import Optional, List, Dict, Any, Literal
from uuid import UUID
from datetime import datetime, date
from schemas.models import *

class get_api_v1_customers_search_Input(BaseModel):
    encryptedParams: Optional[str] = Field(None, description="")

from pydantic import BaseModel
from typing import Optional, Dict, Any
from schemas.models import *

class get_api_v1_customers_search_Output(BaseModel):
    data: Optional[Cortside_AspNetCore_Common_Paging_PagedListOf_Acme_ShoppingCart_WebApi_Models_Responses_CustomerModel] = None