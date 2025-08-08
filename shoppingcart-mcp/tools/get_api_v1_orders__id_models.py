from pydantic import BaseModel, Field
from typing import Optional, List, Dict, Any, Literal
from uuid import UUID
from datetime import datetime, date
from schemas.models import *

class get_api_v1_orders__id_Input(BaseModel):
    id: UUID = Field(..., description="the id of the order to get")

from pydantic import BaseModel
from typing import Optional, Dict, Any
from schemas.models import *

class get_api_v1_orders__id_Output(BaseModel):
    data: Optional[Acme_ShoppingCart_WebApi_Models_Responses_OrderModel] = None