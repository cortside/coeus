import { required } from "@muziehdesign/forms";
import { min, NumberType } from "@muziehdesign/forms";

export class AddToCartModel {
    @NumberType(required(), min(1))
    quantity?: number;
}