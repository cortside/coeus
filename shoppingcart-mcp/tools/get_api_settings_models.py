from pydantic import BaseModel
from typing import Optional
from schemas.models import Acme_ShoppingCart_WebApi_Models_Responses_SettingsModel, Cortside_Health_Models_BuildModel

class get_api_settings_Input(BaseModel):
    pass

class get_api_settings_Output(Acme_ShoppingCart_WebApi_Models_Responses_SettingsModel):
    pass

get_api_settings_Output.model_rebuild()