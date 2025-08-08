from pydantic import BaseModel, Field
from typing import Optional, List, Dict, Any, Literal
from uuid import UUID
from datetime import datetime, date
from schemas.models import *

class post_api_v1_orders__resourceId__publish_Input(BaseModel):
    resourceId: UUID = Field(..., description="")

from pydantic import BaseModel
from typing import Optional, Dict, Any
from schemas.models import *

class post_api_v1_orders__resourceId__publish_Output(BaseModel):
    data: Optional[Dict[str, Any]] = None