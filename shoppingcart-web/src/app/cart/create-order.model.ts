import { ObjectType, required } from "@muziehdesign/forms";
import { AddressInputModel } from "./address-input.model";
import { CustomerInputModel } from "./customer-input.model";

export class CreateOrderModel {
    @ObjectType(CustomerInputModel)
    customer?: CustomerInputModel;
    @ObjectType(AddressInputModel)
    address?: AddressInputModel;
}