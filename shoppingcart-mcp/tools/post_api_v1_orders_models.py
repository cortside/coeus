from pydantic import BaseModel, Field
from typing import Optional, List, Dict, Any, Literal
from uuid import UUID
from datetime import datetime, date
from schemas.models import *

class post_api_v1_orders_Input(BaseModel):
    body: Acme_ShoppingCart_WebApi_Models_Requests_CreateOrderModel = Field(..., description="Request body")