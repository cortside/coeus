namespace Acme.ShoppingCart.DomainService.Tests {
    using System.Threading.Tasks;
    using Acme.ShoppingCart.Data.Repositories;
    using Acme.ShoppingCart.Domain.Entities;
    using Acme.ShoppingCart.DomainService;
    using Cortside.AspNetCore.Common.Models;
    using Moq;
    using Xunit;

    public class CustomerTypeServiceTests {
        private CustomerTypeService _testClass;
        private Mock<ICustomerTypeRepository> _customerRepository;

        public CustomerTypeServiceTests() {
            _customerRepository = new Mock<ICustomerTypeRepository>(MockBehavior.Strict);
            _testClass = new CustomerTypeService(_customerRepository.Object);
        }

        [Fact]
        public async Task CanCallGetCustomerTypesAsync() {
            // Arrange
            _customerRepository.Setup(mock => mock.GetCustomerTypesAsync()).ReturnsAsync(new ListResult<CustomerType> {
                Results = new[] {
                    new CustomerType("TestValue636885736", "TestValue857084813", false),
                    new CustomerType("TestValue974078687", "TestValue1049886686", true),
                    new CustomerType("TestValue190436845", "TestValue1276214411", false)
                }
            });

            // Act
            var result = await _testClass.GetCustomerTypesAsync();

            // Assert
            _customerRepository.Verify(mock => mock.GetCustomerTypesAsync());
            Assert.Equal(3, result.Results.Count);
        }
    }
}
