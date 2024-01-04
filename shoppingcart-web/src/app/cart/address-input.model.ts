import { required, StringType } from "@muziehdesign/forms";

export class AddressInputModel {
    @StringType(required())
    street?: string;
    @StringType(required())
    city?: string;
    @StringType(required())
    state?: string;
    @StringType(required())
    country?: string;
    @StringType(required())
    zipCode?: string;
}