#pragma warning disable CS1591

using System;

namespace Acme.ShoppingCart.WebApi.Models.Responses {
    /// <summary>
    /// Represents a single loan
    /// </summary>
    public class WidgetModel {
        /// <summary>
        /// Unique identifier for a WebApiStarter
        /// </summary>
        public Guid WidgetId { get; set; }
        public string Text { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }
}
