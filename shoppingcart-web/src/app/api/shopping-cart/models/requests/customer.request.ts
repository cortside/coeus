export interface CustomerRequest {
    firstName: string;
    lastName: string;
    email: string;
    birthDate: {
        year: number;
        month: number;
        day: number;
    };
}
