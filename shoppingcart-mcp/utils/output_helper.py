from typing import Any, Type, TypeVar
from pydantic import BaseModel

OutputModelT = TypeVar("OutputModelT", bound=BaseModel)

def create_output(output_model: Type[OutputModelT], res: Any) -> OutputModelT:
    """Construct an output model instance from a raw API response.

    For output models that directly inherit from the response model, just validate and return.
    """
    if isinstance(res, dict):
        try:
            return output_model.model_validate(res)  # Pydantic v2
        except Exception:
            pass
    return output_model()  # fallback to empty instance
