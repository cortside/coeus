from typing import Any, Type, TypeVar
from pydantic import BaseModel

OutputModelT = TypeVar("OutputModelT", bound=BaseModel)

def create_output(output_model: Type[OutputModelT], res: Any) -> OutputModelT:
    """Construct an output model instance from a raw API response.

    Expects the output_model to have a field named "data" annotated with an
    Optional[SomePydanticModel]. If res is a dict and validates against that
    inner model, returns output_model(data=model). Otherwise returns an empty
    output_model() instance.
    """
    if isinstance(res, dict):
        try:
            field = output_model.model_fields.get("data")  # type: ignore[attr-defined]
            if field is not None:
                ann = field.annotation
                # Handle Optional[X] or Union[X, None]
                inner = getattr(ann, "__args__", (ann,))[0]
                if isinstance(inner, type) and issubclass(inner, BaseModel):
                    model = inner.model_validate(res)  # type: ignore[attr-defined]
                    return output_model(data=model)  # type: ignore[arg-type]
        except Exception:
            pass
    return output_model()  # type: ignore[call-arg]
