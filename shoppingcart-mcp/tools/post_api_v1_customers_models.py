from pydantic import BaseModel, Field
from schemas.models import *

class post_api_v1_customers_Input(BaseModel):
    body: Acme_ShoppingCart_WebApi_Models_Requests_UpdateCustomerModel = Field(..., description="Request body")