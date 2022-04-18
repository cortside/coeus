using System.Linq;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Acme.ShoppingCart.WebApi.Filters {
    public class RemoveVersionFromParameter : IOperationFilter {
        public void Apply(OpenApiOperation operation, OperationFilterContext context) {
            if (!operation.Parameters.Any()) {
                return;
            }

            var versionParameter = operation.Parameters.SingleOrDefault(p => p.Name == "version");
            if (versionParameter != null) {
                operation.Parameters.Remove(versionParameter);
            }
        }
    }
}
