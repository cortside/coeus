using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Acme.ShoppingCart.WebApi.Filters {
    public class AuthorizeOperationFilter : IOperationFilter {
        public void Apply(OpenApiOperation operation, OperationFilterContext context) {
            var authorizationAttributes = context.GetControllerAndActionAttributes<AuthorizeAttribute>();
            if (!authorizationAttributes.Any()) {
                return;
            }

            if (operation.Parameters == null) {
                operation.Parameters = new List<OpenApiParameter>();
            }

            // display input parameter Authorization
            operation.Parameters.Add(new OpenApiParameter {
                Name = "Authorization",
                @In = ParameterLocation.Header,
                Description = "access token",
                Required = false,
                Schema = new OpenApiSchema {
                    Type = "string"
                }
            });

            // display possible response status codes
            string code401 = ((int)HttpStatusCode.Unauthorized).ToString();
            if (!operation.Responses.ContainsKey(code401)) {
                operation.Responses.Add(code401, new OpenApiResponse { Description = "Unauthorized" });
            }

            AuthorizeAttribute authorizeAttribute = authorizationAttributes.OfType<AuthorizeAttribute>().FirstOrDefault();
            if (authorizeAttribute != null && !string.IsNullOrWhiteSpace(authorizeAttribute.Policy)) {
                string code403 = ((int)HttpStatusCode.Forbidden).ToString();
                if (!operation.Responses.ContainsKey(code403)) {
                    operation.Responses.Add(code403, new OpenApiResponse { Description = "Forbidden" });
                }
            }

            // display required authentication/policy
            string authorizationDescription = " (Auth)";
            if (authorizeAttribute != null && !string.IsNullOrWhiteSpace(authorizeAttribute.Policy)) {
                authorizationDescription = $" (Auth permission: {authorizeAttribute.Policy})";
            }
            operation.Summary ??= "";
            operation.Summary += authorizationDescription;

            //operation.Description ??= "";
            //operation.Description += authorizationDescription;

            //operation.Security.Add(new OpenApiSecurityRequirement
            //{
            //    { new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "oauth2" } }, new List<string>() }
            //});
        }
    }
}
