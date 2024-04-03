export interface OrderSummaryModel {
    orderResourceId: string;
    status: string;
    customer: CustomerModel;
}

export interface CustomerModel {
    customerResourceId: string;
    firstName: string;
    lastName: string;
    email: string;
}

export interface CreateOrderResultModel {
    orderResourceId: string;
    status: string;
}