import { AuditableResponse } from '../../../auditable.response';

export interface CustomerResponse extends AuditableResponse {
    customerResourceId: string;
    firstName: string;
    lastName: string;
    email: string;
}
