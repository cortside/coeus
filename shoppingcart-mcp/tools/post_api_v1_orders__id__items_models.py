from pydantic import BaseModel, Field
from typing import Optional, List, Dict, Any, Literal
from uuid import UUID
from datetime import datetime, date
from schemas.models import *

class post_api_v1_orders__id__items_Input(BaseModel):
    id: UUID = Field(..., description="")
    body: Acme_ShoppingCart_WebApi_Models_Requests_CreateOrderItemModel = Field(..., description="Request body")

from pydantic import BaseModel
from typing import Optional, Dict, Any
from schemas.models import *

class post_api_v1_orders__id__items_Output(BaseModel):
    data: Optional[Acme_ShoppingCart_WebApi_Models_Responses_OrderModel] = None