using System.Collections.Generic;
using Acme.IdentityServer.WebApi.Models;

namespace Acme.IdentityServer.WebApi.Helpers {
    public interface IHtmlTemplateHelper {
        string GetHtmlTemplate(HtmlTemplateType templateType);
        string GetHtmlTemplate(HtmlTemplateType templateType, List<HtmlTemplateParameter> tokens);
    }
}
