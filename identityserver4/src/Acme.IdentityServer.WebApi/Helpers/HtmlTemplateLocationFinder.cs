using Acme.IdentityServer.WebApi.Models;

namespace Acme.IdentityServer.WebApi.Helpers {
    public class HtmlTemplateLocationFinder : IHtmlTemplateLocationFinder {
        public string GetLocation(HtmlTemplateType templateType) {
            if (templateType == HtmlTemplateType.ClientSecretEmail) {
                return "ClientSecretEmailHtmlTemplate.html";
            }

            return string.Empty;
        }
    }
}
