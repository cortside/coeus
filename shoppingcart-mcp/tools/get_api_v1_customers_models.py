from pydantic import BaseModel, Field
from typing import Optional, List, Dict, Any, Literal
from uuid import UUID
from datetime import datetime, date
from schemas.models import *

class get_api_v1_customers_Input(BaseModel):
    CustomerResourceId: Optional[UUID] = Field(None, description="Gets or sets the customer resource identifier.")
    FirstName: Optional[str] = Field(None, description="Gets or sets the first name.")
    LastName: Optional[str] = Field(None, description="Gets or sets the last name.")
    PageNumber: Optional[int] = Field(None, description="")
    PageSize: Optional[int] = Field(None, description="")
    Sort: Optional[str] = Field(None, description="")

from pydantic import BaseModel
from typing import Optional, Dict, Any
from schemas.models import *

class get_api_v1_customers_Output(BaseModel):
    data: Optional[Cortside_AspNetCore_Common_Paging_PagedListOf_Acme_ShoppingCart_WebApi_Models_Responses_CustomerModel] = None