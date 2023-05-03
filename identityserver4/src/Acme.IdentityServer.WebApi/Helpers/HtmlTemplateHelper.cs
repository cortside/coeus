using System.Collections.Generic;
using System.IO.Abstractions;
using System.Text;
using System.Text.Encodings.Web;
using Acme.IdentityServer.WebApi.Models;
using Microsoft.Extensions.Configuration;

namespace Acme.IdentityServer.WebApi.Helpers {

    /// <summary>
    /// Helps with handling static html templates
    /// </summary>
    public class HtmlTemplateHelper : IHtmlTemplateHelper {
        private readonly IConfigurationRoot config;
        private readonly IFileSystem fileSystem;
        private readonly IHtmlTemplateLocationFinder htmlTemplateLocationFinder;

        public HtmlTemplateHelper(IConfigurationRoot config, IFileSystem fileSystem, IHtmlTemplateLocationFinder htmlTemplateLocationFinder) {
            this.config = config;
            this.fileSystem = fileSystem;
            this.htmlTemplateLocationFinder = htmlTemplateLocationFinder;
        }

        /// <summary>
        /// Get Html Template by Template Type with replacing tokenized values
        /// </summary>
        /// <param name="templateType"></param>
        /// <param name="tokens"></param>
        /// <returns></returns>
        public string GetHtmlTemplate(HtmlTemplateType templateType, List<HtmlTemplateParameter> tokens) {
            var sb = new StringBuilder(GetHtmlBody(templateType));

            tokens?.ForEach(token => {
                sb.Replace(token.ParameterName, HtmlEncoder.Default.Encode(token.ParameterValue));
            });

            return sb.ToString();
        }

        /// <summary>
        /// Get Html Template by Template Type without tokenized values
        /// </summary>
        /// <param name="templateType"></param>
        /// <returns></returns>
        public string GetHtmlTemplate(HtmlTemplateType templateType) {
            return GetHtmlBody(templateType);
        }

        private string GetHtmlBody(HtmlTemplateType templateType) {
            var defaultTemplateLocation = config["HtmlTemplateLocation"];
            var templateLocation = htmlTemplateLocationFinder.GetLocation(templateType);
            var fullTemplateLocation = defaultTemplateLocation + templateLocation;

            return fileSystem.File.ReadAllText(fullTemplateLocation);
        }
    }
}
