from pydantic import BaseModel, Field
from typing import Optional, List, Dict, Any, Literal
from uuid import UUID
from datetime import datetime, date
from schemas.models import *

class get_api_health_Input(BaseModel):
    pass

from pydantic import BaseModel
from typing import Optional, Dict, Any
from schemas.models import *

class get_api_health_Output(BaseModel):
    data: Optional[Cortside_Health_Models_HealthModel] = None