#pragma warning disable CS1591 // Missing XML comments

using Acme.ShoppingCart.Dto;
using Acme.ShoppingCart.Dto.Input;
using Acme.ShoppingCart.Dto.Output;
using Acme.ShoppingCart.Dto.Search;
using Acme.ShoppingCart.WebApi.Models.Enumerations;
using Acme.ShoppingCart.WebApi.Models.Requests;
using Acme.ShoppingCart.WebApi.Models.Responses;

namespace Acme.ShoppingCart.WebApi.Mappers {
    public class OrderModelMapper {
        private readonly CustomerModelMapper customerMapper;
        private readonly AddressModelMapper addressMapper;
        private readonly SubjectModelMapper subjectMapper;

        public OrderModelMapper(CustomerModelMapper customerMapper, AddressModelMapper addressMapper, SubjectModelMapper subjectMapper) {
            this.customerMapper = customerMapper;
            this.addressMapper = addressMapper;
            this.subjectMapper = subjectMapper;
        }

        public OrderModel Map(OrderDto dto) {
            if (dto == null) {
                return null;
            }

            var model = new OrderModel() {
                OrderResourceId = dto.OrderResourceId,
                Address = addressMapper.Map(dto.Address),
                Items = dto.Items.ConvertAll(x => Map(x)),
                Customer = customerMapper.Map(dto.Customer),
                Status = ((OrderStatus)(int)dto.Status),
                CreatedDate = dto.CreatedDate,
                LastModifiedDate = dto.LastModifiedDate,
                CreatedSubject = subjectMapper.Map(dto.CreatedSubject),
                LastModifiedSubject = subjectMapper.Map(dto.LastModifiedSubject)
            };

            return model;
        }

        public OrderItemModel Map(OrderItemDto dto) {
            if (dto == null) {
                return null;
            }

            return new OrderItemModel {
                OrderItemId = dto.OrderItemId,
                ItemId = dto.ItemId,
                Sku = dto.Sku,
                Quantity = dto.Quantity,
                UnitPrice = dto.UnitPrice
            };
        }

        public OrderSearchDto MapToDto(OrderSearchModel model) {
            if (model == null) {
                return null;
            }

            return new OrderSearchDto() {
                CustomerResourceId = model.CustomerResourceId,
                FirstName = model.FirstName,
                LastName = model.LastName,
                PageNumber = model.PageNumber,
                PageSize = model.PageSize,
                Sort = model.Sort
            };
        }

        public CreateOrderDto MapToDto(CreateOrderModel model) {
            if (model == null) {
                return null;
            }

            return new CreateOrderDto() {
                Customer = new CreateOrderCustomerDto() {
                    FirstName = model.Customer.FirstName,
                    LastName = model.Customer.LastName,
                    Email = model.Customer.Email
                },
                Address = new AddressDto() {
                    Street = model.Address.Street,
                    City = model.Address.City,
                    State = model.Address.State,
                    Country = model.Address.Country,
                    ZipCode = model.Address.ZipCode
                },
                Items = model.Items?.ConvertAll(x => new UpdateOrderItemDto() { Sku = x.Sku, Quantity = x.Quantity.Value })
            };
        }

        public CreateOrderDto MapToDto(CreateCustomerOrderModel model) {
            if (model == null) {
                return null;
            }

            return new CreateOrderDto() {
                Customer = new CreateOrderCustomerDto(),
                Address = new AddressDto() {
                    Street = model.Address.Street,
                    City = model.Address.City,
                    State = model.Address.State,
                    Country = model.Address.Country,
                    ZipCode = model.Address.ZipCode
                },
                Items = model.Items?.ConvertAll(x => new UpdateOrderItemDto() { Sku = x.Sku, Quantity = x.Quantity.Value })
            };
        }

        public UpdateOrderDto MapToDto(UpdateOrderModel model) {
            if (model == null) {
                return null;
            }

            return new UpdateOrderDto() {
                Address = new AddressDto() {
                    Street = model.Address.Street,
                    City = model.Address.City,
                    State = model.Address.State,
                    Country = model.Address.Country,
                    ZipCode = model.Address.ZipCode
                },
                Items = model.Items?.ConvertAll(x => new UpdateOrderItemDto() { Sku = x.Sku, Quantity = x.Quantity.Value })
            };
        }

        public OrderItemDto MapToDto(CreateOrderItemModel model) {
            if (model == null) {
                return null;
            }

            return new OrderItemDto() {
                Sku = model.Sku,
                Quantity = model.Quantity.Value
            };
        }
    }
}
