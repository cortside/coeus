import { SubjectResponse } from "./subject.response";

export interface AuditableResponse {
    createdDate: Date;
    lastModifiedDate: Date;
    createdSubject: SubjectResponse;
    lastModifiedSubject: SubjectResponse;
}
