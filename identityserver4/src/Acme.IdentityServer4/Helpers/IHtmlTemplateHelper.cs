using System.Collections.Generic;
using Cortside.IdentityServer.WebApi.Models;

namespace Cortside.IdentityServer.WebApi.Helpers {
    public interface IHtmlTemplateHelper {
        string GetHtmlTemplate(HtmlTemplateType templateType);
        string GetHtmlTemplate(HtmlTemplateType templateType, List<HtmlTemplateParameter> tokens);
    }
}
