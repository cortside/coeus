using System;
using System.Threading.Tasks;
using Acme.ShoppingCart.Data;
using Acme.ShoppingCart.DomainService;
using Moq;
using Xunit;

namespace Acme.ShoppingCart.Facade.Tests {
    public class CustomerFacadeTest {
        [Fact]
        public async Task ShouldGetCustomer() {
            // arrange
            var uow = new Mock<IUnitOfWork>();
            var customerService = new Mock<ICustomerService>();
            var facade = new CustomerFacade(uow.Object, customerService.Object, new Mappers.CustomerMapper(new Mappers.SubjectMapper()));
            var customerResourceId = Guid.NewGuid();
            customerService.Setup(x => x.GetCustomerAsync(customerResourceId)).ReturnsAsync(new Domain.Entities.Customer("elmer", "fudd", "elmer.fudd@gmail.org"));

            // act
            var customer = await facade.GetCustomerAsync(customerResourceId);

            // assert
            Assert.NotNull(customer);
        }
    }
}
