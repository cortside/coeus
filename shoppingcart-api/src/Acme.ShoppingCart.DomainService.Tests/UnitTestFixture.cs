using System.Collections.Generic;
using Moq;

namespace Acme.ShoppingCart.DomainService.Tests {
    public class UnitTestFixture {
        private readonly List<Mock> mocks;

        public UnitTestFixture() {
            mocks = new List<Mock>();
        }

        public Mock<T> Mock<T>() where T : class {
            var mock = new Mock<T>();
            this.mocks.Add(mock);
            return mock;
        }

        public void TearDown() {
            this.mocks.ForEach(m => m.VerifyAll());
        }
    }
}
