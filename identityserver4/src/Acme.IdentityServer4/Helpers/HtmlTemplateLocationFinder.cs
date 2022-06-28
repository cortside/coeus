using EnerBank.IdentityServer.WebApi.Models;

namespace EnerBank.IdentityServer.WebApi.Helpers {
    public class HtmlTemplateLocationFinder : IHtmlTemplateLocationFinder {
        public string GetLocation(HtmlTemplateType templateType) {
            if (templateType == HtmlTemplateType.ClientSecretEmail) {
                return "ClientSecretEmailHtmlTemplate.html";
            }

            return string.Empty;
        }
    }
}
