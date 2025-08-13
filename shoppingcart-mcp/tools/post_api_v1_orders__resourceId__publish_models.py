from pydantic import BaseModel, Field
from uuid import UUID
from schemas.models import *

class post_api_v1_orders__resourceId__publish_Input(BaseModel):
    resourceId: UUID = Field(..., description="the order id")