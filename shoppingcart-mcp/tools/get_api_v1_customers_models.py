from pydantic import BaseModel, Field
from typing import Optional
from schemas.models import *

class get_api_v1_customers_Input(BaseModel):
    CustomerResourceId: Optional[UUID] = Field(None, description="Gets or sets the customer resource identifier.")
    FirstName: Optional[str] = Field(None, description="Gets or sets the first name.")
    LastName: Optional[str] = Field(None, description="Gets or sets the last name.")
    PageNumber: Optional[int] = Field(None, description="")
    PageSize: Optional[int] = Field(None, description="")
    Sort: Optional[str] = Field(None, description="")