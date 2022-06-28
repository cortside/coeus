using EnerBank.IdentityServer.WebApi.Models;

namespace EnerBank.IdentityServer.WebApi.Helpers {
    public interface IHtmlTemplateLocationFinder {
        string GetLocation(HtmlTemplateType templateType);
    }
}