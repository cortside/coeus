using System.Collections.Generic;
using EnerBank.IdentityServer.WebApi.Models;

namespace EnerBank.IdentityServer.WebApi.Helpers {
    public interface IHtmlTemplateHelper {
        string GetHtmlTemplate(HtmlTemplateType templateType);
        string GetHtmlTemplate(HtmlTemplateType templateType, List<HtmlTemplateParameter> tokens);
    }
}
