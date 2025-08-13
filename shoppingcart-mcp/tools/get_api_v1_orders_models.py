from pydantic import BaseModel, Field
from typing import Optional
from uuid import UUID
from schemas.models import *

class get_api_v1_orders_Input(BaseModel):
    CustomerResourceId: Optional[UUID] = Field(None, description="Customer's identifier")
    FirstName: Optional[str] = Field(None, description="Customer first name")
    LastName: Optional[str] = Field(None, description="Customer last name")
    PageNumber: Optional[int] = Field(None, description="")
    PageSize: Optional[int] = Field(None, description="")
    Sort: Optional[str] = Field(None, description="")