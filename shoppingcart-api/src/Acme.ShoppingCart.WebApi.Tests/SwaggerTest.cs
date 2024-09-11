using System;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace Acme.ShoppingCart.WebApi.Tests {
    public class SwaggerTest {
        [Fact]
        public void Foo() {
            var controllers = typeof(Startup)
                .Assembly
                .GetTypes()
                .Where(t => typeof(Microsoft.AspNetCore.Mvc.Controller).IsAssignableFrom(t))
                .ToList();

            var success = true;
            var message = string.Empty;
            foreach (var controller in controllers) {
                foreach (var method in controller.GetMethods(BindingFlags.Public | BindingFlags.Instance).Where(x => x.DeclaringType == controller)) {
                    var attributes = method.GetCustomAttributes(false);
                    var hasResponseType = Array.Exists(attributes, a => a.GetType() == typeof(ProducesResponseTypeAttribute));
                    if (!hasResponseType) {
                        success = false;
                        message += controller.Name + "::" + method.Name + Environment.NewLine;
                    }
                }
            }

            Assert.True(success, message);
        }
    }
}
