from pydantic import BaseModel, Field
from typing import Optional, List, Dict, Any, Literal
from uuid import UUID
from datetime import datetime, date
from schemas.models import *

class post_api_v1_customers__resourceId__orders_Input(BaseModel):
    resourceId: UUID = Field(..., description="")
    body: Acme_ShoppingCart_WebApi_Models_Requests_CreateCustomerOrderModel = Field(..., description="Request body")

from pydantic import BaseModel
from typing import Optional, Dict, Any
from schemas.models import *

class post_api_v1_customers__resourceId__orders_Output(BaseModel):
    data: Optional[Acme_ShoppingCart_WebApi_Models_Responses_OrderModel] = None