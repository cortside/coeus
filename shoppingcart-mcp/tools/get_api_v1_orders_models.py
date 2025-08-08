from pydantic import BaseModel, Field
from typing import Optional, List, Dict, Any, Literal
from uuid import UUID
from datetime import datetime, date
from schemas.models import *

class get_api_v1_orders_Input(BaseModel):
    CustomerResourceId: Optional[UUID] = Field(None, description="Customer's identifier")
    FirstName: Optional[str] = Field(None, description="Customer first name")
    LastName: Optional[str] = Field(None, description="Customer last name")
    PageNumber: Optional[int] = Field(None, description="")
    PageSize: Optional[int] = Field(None, description="")
    Sort: Optional[str] = Field(None, description="")

from pydantic import BaseModel
from typing import Optional, Dict, Any
from schemas.models import *

class get_api_v1_orders_Output(BaseModel):
    data: Optional[Cortside_AspNetCore_Common_Paging_PagedListOf_Acme_ShoppingCart_WebApi_Models_Responses_OrderModel] = None