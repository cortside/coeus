using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Acme.ShoppingCart.WebApi.Tests {
    public abstract class ControllerTest<T> : IDisposable where T : ControllerBase {
        protected T controller;
        protected UnitTestFixture testFixture;

        protected ControllerTest() {
            testFixture = new UnitTestFixture();
        }

        protected ControllerContext GetControllerContext() {
            var controllerContext = new ControllerContext {
                HttpContext = new DefaultHttpContext()
            };
            return controllerContext;
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) {
            // Cleanup
            testFixture.TearDown();
        }
    }
}
