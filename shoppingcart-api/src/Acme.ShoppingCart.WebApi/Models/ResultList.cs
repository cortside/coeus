using System.Collections.Generic;

namespace Acme.ShoppingCart.WebApi.Models {
    /// <summary>
    /// List of results
    /// </summary>
    /// <typeparam name="T">model</typeparam>
    public class ResultList<T> {
        /// <summary>
        /// Results
        /// </summary>
        public IList<T> Results { get; set; }
    }
}
