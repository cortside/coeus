using System.Collections.Generic;
using Moq;

namespace Acme.ShoppingCart.DomainService.Tests {
    public class UnitTestFixture {
        private readonly List<Mock> mocks;

        public UnitTestFixture() {
            mocks = [];
        }

        public Mock<T> Mock<T>() where T : class {
            var mock = new Mock<T>();
            mocks.Add(mock);
            return mock;
        }

        public void TearDown() {
            mocks.ForEach(m => m.VerifyAll());
        }
    }
}
