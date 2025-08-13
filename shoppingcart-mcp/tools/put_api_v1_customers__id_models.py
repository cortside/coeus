from pydantic import BaseModel, Field
from typing import Optional, List, Dict, Any, Literal
from uuid import UUID
from datetime import datetime, date
from schemas.models import *

class put_api_v1_customers__id_Input(BaseModel):
    id: UUID = Field(..., description="")
    body: Acme_ShoppingCart_WebApi_Models_Requests_UpdateCustomerModel = Field(..., description="Request body")