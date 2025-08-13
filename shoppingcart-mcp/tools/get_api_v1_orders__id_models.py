from pydantic import BaseModel, Field
from typing import Optional, List, Dict, Any, Literal
from uuid import UUID
from datetime import datetime, date
from schemas.models import *

class get_api_v1_orders__id_Input(BaseModel):
    id: UUID = Field(..., description="the id of the order to get")