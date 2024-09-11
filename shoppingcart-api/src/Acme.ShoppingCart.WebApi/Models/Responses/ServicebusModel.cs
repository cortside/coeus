namespace Acme.ShoppingCart.WebApi.Models.Responses {
    /// <summary>
    /// Service bus model
    /// </summary>
    public class ServiceBusModel {
        /// <summary>
        /// Name Space
        /// </summary>
        public string NameSpace { get; set; }

        /// <summary>
        /// Queue
        /// </summary>
        public string Queue { get; set; }

        /// <summary>
        /// Exchange
        /// </summary>
        public string Exchange { get; set; }
    }
}
