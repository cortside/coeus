using Acme.IdentityServer.WebApi.Models;

namespace Acme.IdentityServer.WebApi.Helpers {
    public interface IHtmlTemplateLocationFinder {
        string GetLocation(HtmlTemplateType templateType);
    }
}
