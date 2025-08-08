from __future__ import annotations
from pydantic import BaseModel, Field
from typing import Optional, List, Dict, Any, Literal
from uuid import UUID
from datetime import datetime, date

Acme_ShoppingCart_Enumerations_OrderStatus = Literal['created', 'paid', 'shipped', 'cancelled']

class Acme_ShoppingCart_WebApi_Models_AddressModel(BaseModel):
    street: Optional[str] = Field(None, description="Gets or sets the street.")
    city: Optional[str] = Field(None, description="Gets or sets the city.")
    state: Optional[str] = Field(None, description="Gets or sets the state.")
    country: Optional[str] = Field(None, description="Gets or sets the country.")
    zipCode: Optional[str] = Field(None, description="Gets or sets the zip code.")

class Acme_ShoppingCart_WebApi_Models_Requests_CreateCustomerOrderModel(BaseModel):
    address: Acme_ShoppingCart_WebApi_Models_AddressModel = Field(..., description="")
    items: Optional[List[Acme_ShoppingCart_WebApi_Models_Requests_CreateOrderItemModel]] = Field(None, description="Gets or sets the items.")

class Acme_ShoppingCart_WebApi_Models_Requests_CreateOrderItemModel(BaseModel):
    sku: Optional[str] = Field(None, description="Gets or sets the sku.")
    quantity: Optional[int] = Field(None, description="Gets or sets the quantity.")

class Acme_ShoppingCart_WebApi_Models_Requests_CreateOrderModel(BaseModel):
    customer: Acme_ShoppingCart_WebApi_Models_Requests_UpdateOrderCustomerModel = Field(..., description="")
    address: Acme_ShoppingCart_WebApi_Models_AddressModel = Field(..., description="")
    items: Optional[List[Acme_ShoppingCart_WebApi_Models_Requests_CreateOrderItemModel]] = Field(None, description="Gets or sets the items.")

class Acme_ShoppingCart_WebApi_Models_Requests_CustomerSearchModel(BaseModel):
    customerResourceId: Optional[UUID] = Field(None, description="Gets or sets the customer resource identifier.")
    firstName: Optional[str] = Field(None, description="Gets or sets the first name.")
    lastName: Optional[str] = Field(None, description="Gets or sets the last name.")
    pageNumber: Optional[int] = Field(None, description="")
    pageSize: Optional[int] = Field(None, description="")
    sort: Optional[str] = Field(None, description="")

class Acme_ShoppingCart_WebApi_Models_Requests_UpdateCustomerModel(BaseModel):
    firstName: str = Field(..., description="Gets or sets the first name.")
    lastName: str = Field(..., description="Gets or sets the last name.")
    email: str = Field(..., description="Gets or sets the email.")
    birthDate: date = Field(..., description="Gets or sets the email.")

class Acme_ShoppingCart_WebApi_Models_Requests_UpdateOrderCustomerModel(BaseModel):
    customerResourceId: Optional[UUID] = Field(None, description="Gets or sets the customer resource identifier.")
    firstName: str = Field(..., description="Gets or sets the first name.")
    lastName: str = Field(..., description="Gets or sets the last name.")
    email: str = Field(..., description="Gets or sets the email.")
    birthDate: date = Field(..., description="Gets or sets the email.")

class Acme_ShoppingCart_WebApi_Models_Requests_UpdateOrderModel(BaseModel):
    address: Acme_ShoppingCart_WebApi_Models_AddressModel = Field(..., description="")
    items: Optional[List[Acme_ShoppingCart_WebApi_Models_Requests_CreateOrderItemModel]] = Field(None, description="Gets or sets the items.")

class Acme_ShoppingCart_WebApi_Models_Responses_AuthorizationModel(BaseModel):
    roles: Optional[List[str]] = Field(None, description="Roles")
    permissions: Optional[List[str]] = Field(None, description="Permissions")
    principal: Optional[Cortside_Common_Security_SubjectPrincipal] = Field(None, description="")

class Acme_ShoppingCart_WebApi_Models_Responses_ConfigurationModel(BaseModel):
    serviceBus: Optional[Acme_ShoppingCart_WebApi_Models_Responses_ServiceBusModel] = Field(None, description="")
    identityServer: Optional[Acme_ShoppingCart_WebApi_Models_Responses_IdentityServerModel] = Field(None, description="")
    policyServer: Optional[Acme_ShoppingCart_WebApi_Models_Responses_PolicyServerModel] = Field(None, description="")

class Acme_ShoppingCart_WebApi_Models_Responses_CustomerModel(BaseModel):
    customerResourceId: Optional[UUID] = Field(None, description="Gets or sets the customer resource identifier.")
    firstName: Optional[str] = Field(None, description="Gets or sets the first name.")
    lastName: Optional[str] = Field(None, description="Gets or sets the last name.")
    email: Optional[str] = Field(None, description="Gets or sets the email.")
    createdDate: Optional[datetime] = Field(None, description="")
    createdSubject: Optional[Cortside_AspNetCore_Common_Models_SubjectModel] = Field(None, description="")
    lastModifiedDate: Optional[datetime] = Field(None, description="")
    lastModifiedSubject: Optional[Cortside_AspNetCore_Common_Models_SubjectModel] = Field(None, description="")

class Acme_ShoppingCart_WebApi_Models_Responses_IdentityServerModel(BaseModel):
    authority: Optional[str] = Field(None, description="Authority")
    apiname: Optional[str] = Field(None, description="ApiName")
    baseUrl: Optional[str] = Field(None, description="Base Url")

class Acme_ShoppingCart_WebApi_Models_Responses_OrderItemModel(BaseModel):
    orderItemId: Optional[int] = Field(None, description="Gets or sets the order item identifier.")
    itemId: Optional[UUID] = Field(None, description="Gets or sets the item identifier.")
    sku: Optional[str] = Field(None, description="Gets or sets the sku.")
    quantity: Optional[int] = Field(None, description="Gets or sets the quantity.")
    unitPrice: Optional[float] = Field(None, description="Gets or sets the unit price.")
    createdDate: Optional[datetime] = Field(None, description="")
    createdSubject: Optional[Cortside_AspNetCore_Common_Models_SubjectModel] = Field(None, description="")
    lastModifiedDate: Optional[datetime] = Field(None, description="")
    lastModifiedSubject: Optional[Cortside_AspNetCore_Common_Models_SubjectModel] = Field(None, description="")

class Acme_ShoppingCart_WebApi_Models_Responses_OrderModel(BaseModel):
    orderResourceId: Optional[UUID] = Field(None, description="Gets or sets the order resource identifier.")
    status: Optional[Acme_ShoppingCart_Enumerations_OrderStatus] = Field(None, description="")
    customer: Optional[Acme_ShoppingCart_WebApi_Models_Responses_CustomerModel] = Field(None, description="")
    address: Optional[Acme_ShoppingCart_WebApi_Models_AddressModel] = Field(None, description="")
    items: Optional[List[Acme_ShoppingCart_WebApi_Models_Responses_OrderItemModel]] = Field(None, description="Gets or sets the items.")
    createdDate: Optional[datetime] = Field(None, description="")
    createdSubject: Optional[Cortside_AspNetCore_Common_Models_SubjectModel] = Field(None, description="")
    lastModifiedDate: Optional[datetime] = Field(None, description="")
    lastModifiedSubject: Optional[Cortside_AspNetCore_Common_Models_SubjectModel] = Field(None, description="")

class Acme_ShoppingCart_WebApi_Models_Responses_PolicyServerModel(BaseModel):
    basePolicy: Optional[str] = Field(None, description="Base Policys")
    url: Optional[str] = Field(None, description="Url")

class Acme_ShoppingCart_WebApi_Models_Responses_ServiceBusModel(BaseModel):
    nameSpace: Optional[str] = Field(None, description="Name Space")
    queue: Optional[str] = Field(None, description="Queue")
    exchange: Optional[str] = Field(None, description="Exchange")

class Acme_ShoppingCart_WebApi_Models_Responses_SettingsModel(BaseModel):
    service: Optional[str] = Field(None, description="Which service the settings came from")
    build: Optional[Cortside_Health_Models_BuildModel] = Field(None, description="")
    configuration: Optional[Acme_ShoppingCart_WebApi_Models_Responses_ConfigurationModel] = Field(None, description="")

class Cortside_AspNetCore_Common_Models_ErrorModel(BaseModel):
    type: Optional[str] = Field(None, description="")
    property: Optional[str] = Field(None, description="")
    message: Optional[str] = Field(None, description="")
    exception: Optional[Any] = Field(None, description="")

class Cortside_AspNetCore_Common_Models_ErrorsModel(BaseModel):
    errors: Optional[List[Cortside_AspNetCore_Common_Models_ErrorModel]] = Field(None, description="")

class Cortside_AspNetCore_Common_Models_SubjectModel(BaseModel):
    subjectId: Optional[UUID] = Field(None, description="")
    name: Optional[str] = Field(None, description="")
    givenName: Optional[str] = Field(None, description="")
    familyName: Optional[str] = Field(None, description="")
    userPrincipalName: Optional[str] = Field(None, description="")

class Cortside_AspNetCore_Common_Paging_PagedListOf_Acme_ShoppingCart_WebApi_Models_Responses_CustomerModel(BaseModel):
    totalItems: Optional[int] = Field(None, description="")
    pageNumber: Optional[int] = Field(None, description="")
    pageSize: Optional[int] = Field(None, description="")
    totalPages: Optional[int] = Field(None, description="")
    items: Optional[List[Acme_ShoppingCart_WebApi_Models_Responses_CustomerModel]] = Field(None, description="")

class Cortside_AspNetCore_Common_Paging_PagedListOf_Acme_ShoppingCart_WebApi_Models_Responses_OrderModel(BaseModel):
    totalItems: Optional[int] = Field(None, description="")
    pageNumber: Optional[int] = Field(None, description="")
    pageSize: Optional[int] = Field(None, description="")
    totalPages: Optional[int] = Field(None, description="")
    items: Optional[List[Acme_ShoppingCart_WebApi_Models_Responses_OrderModel]] = Field(None, description="")

class Cortside_Common_Security_SubjectClaim(BaseModel):
    type: Optional[str] = Field(None, description="")
    value: Optional[str] = Field(None, description="")

class Cortside_Common_Security_SubjectPrincipal(BaseModel):
    subjectId: Optional[str] = Field(None, description="")
    name: Optional[str] = Field(None, description="")
    givenName: Optional[str] = Field(None, description="")
    familyName: Optional[str] = Field(None, description="")
    userPrincipalName: Optional[str] = Field(None, description="")
    actor: Optional[Cortside_Common_Security_SubjectPrincipal] = Field(None, description="")
    claims: Optional[List[Cortside_Common_Security_SubjectClaim]] = Field(None, description="")

Cortside_Health_Enums_ServiceStatus = Literal['ok', 'degraded', 'failure']

class Cortside_Health_Models_Availability(BaseModel):
    count: Optional[int] = Field(None, description="")
    success: Optional[int] = Field(None, description="")
    failure: Optional[int] = Field(None, description="")
    uptime: Optional[float] = Field(None, description="")
    totalDuration: Optional[int] = Field(None, description="")
    averageDuration: Optional[float] = Field(None, description="")
    lastSuccess: Optional[datetime] = Field(None, description="")
    lastFailure: Optional[datetime] = Field(None, description="")

class Cortside_Health_Models_BuildModel(BaseModel):
    timestamp: Optional[datetime] = Field(None, description="")
    version: Optional[str] = Field(None, description="")
    tag: Optional[str] = Field(None, description="")
    suffix: Optional[str] = Field(None, description="")

class Cortside_Health_Models_HealthModel(BaseModel):
    service: Optional[str] = Field(None, description="")
    build: Optional[Cortside_Health_Models_BuildModel] = Field(None, description="")
    checks: Optional[Dict[str, Cortside_Health_Models_ServiceStatusModel]] = Field(None, description="")
    uptime: Optional[str] = Field(None, description="")
    healthy: Optional[bool] = Field(None, description="")
    status: Optional[Cortside_Health_Enums_ServiceStatus] = Field(None, description="")
    statusDetail: Optional[str] = Field(None, description="")
    timestamp: Optional[datetime] = Field(None, description="")
    required: Optional[bool] = Field(None, description="")
    availability: Optional[Cortside_Health_Models_Availability] = Field(None, description="")

class Cortside_Health_Models_ServiceStatusModel(BaseModel):
    healthy: Optional[bool] = Field(None, description="")
    status: Optional[Cortside_Health_Enums_ServiceStatus] = Field(None, description="")
    statusDetail: Optional[str] = Field(None, description="")
    timestamp: Optional[datetime] = Field(None, description="")
    required: Optional[bool] = Field(None, description="")
    availability: Optional[Cortside_Health_Models_Availability] = Field(None, description="")
