import { required, StringType } from "@muziehdesign/forms";

export class CustomerInputModel {
    
    @StringType(required())
    firstName?: string;
    @StringType(required())
    lastName?: string;
    @StringType(required())
    emailAddress?: string;
    @StringType(required())
    birthdate?: string;
}