using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Acme.ShoppingCart.WebApi.Filters {
    public class ReplaceVersionWithExactValueInPath : IDocumentFilter {
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context) {
            var paths = new OpenApiPaths();

            foreach (var (key, value) in swaggerDoc.Paths)
                paths.Add(key.Replace("v{version}", swaggerDoc.Info.Version), value);

            swaggerDoc.Paths = paths;
        }
    }
}
