from pydantic import BaseModel, Field
from typing import Optional, List, Dict, Any, Literal
from uuid import UUID
from datetime import datetime, date
from schemas.models import *

class get_api_settings_Input(BaseModel):
    pass

from pydantic import BaseModel
from typing import Optional, Dict, Any
from schemas.models import *

class get_api_settings_Output(BaseModel):
    data: Optional[Acme_ShoppingCart_WebApi_Models_Responses_SettingsModel] = None