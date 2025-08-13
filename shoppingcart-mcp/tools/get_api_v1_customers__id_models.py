from pydantic import BaseModel, Field
from uuid import UUID
from schemas.models import *

class get_api_v1_customers__id_Input(BaseModel):
    id: UUID = Field(..., description="the id of the customer to get")