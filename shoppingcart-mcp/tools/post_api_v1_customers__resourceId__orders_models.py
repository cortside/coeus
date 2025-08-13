from pydantic import BaseModel, Field
from uuid import UUID
from schemas.models import *

class post_api_v1_customers__resourceId__orders_Input(BaseModel):
    resourceId: UUID = Field(..., description="the customer id")
    body: Acme_ShoppingCart_WebApi_Models_Requests_CreateOrderModel = Field(..., description="Request body")