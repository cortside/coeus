using Cortside.IdentityServer.WebApi.Models;

namespace Cortside.IdentityServer.WebApi.Helpers {
    public interface IHtmlTemplateLocationFinder {
        string GetLocation(HtmlTemplateType templateType);
    }
}