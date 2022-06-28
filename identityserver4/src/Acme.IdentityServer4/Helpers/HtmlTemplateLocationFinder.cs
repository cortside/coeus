using Cortside.IdentityServer.WebApi.Models;

namespace Cortside.IdentityServer.WebApi.Helpers {
    public class HtmlTemplateLocationFinder : IHtmlTemplateLocationFinder {
        public string GetLocation(HtmlTemplateType templateType) {
            if (templateType == HtmlTemplateType.ClientSecretEmail) {
                return "ClientSecretEmailHtmlTemplate.html";
            }

            return string.Empty;
        }
    }
}
