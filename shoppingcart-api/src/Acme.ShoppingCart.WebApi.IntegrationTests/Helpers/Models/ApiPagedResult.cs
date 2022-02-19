using System.Collections.Generic;
using Newtonsoft.Json;

namespace Acme.ShoppingCart.WebApi.IntegrationTests.Helpers.Models {
    public partial class ApiPagedResult<T> {
        /// <summary>
        /// Initializes a new instance of the
        /// PagedResultTest
        /// class.
        /// </summary>
        public ApiPagedResult() {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the
        /// PagedResultTest
        /// class.
        /// </summary>
        /// <param name="totalItems">Total number of items found for the given
        /// resource</param>
        /// <param name="pageNumber">Current paged result page number</param>
        /// <param name="pageSize">Current paged result page size</param>
        /// <param name="totalPages">Total number of paged result pages for
        /// given resource</param>
        /// <param name="results">Current paged result page of results</param>
        public ApiPagedResult(int? totalItems = default, int? pageNumber = default, int? pageSize = default, int? totalPages = default, IList<T> results = default) {
            TotalItems = totalItems;
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalPages = totalPages;
            Results = results;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// Gets total number of items found for the given resource
        /// </summary>
        [JsonProperty(PropertyName = "totalItems")]
        public int? TotalItems { get; private set; }

        /// <summary>
        /// Gets current paged result page number
        /// </summary>
        [JsonProperty(PropertyName = "pageNumber")]
        public int? PageNumber { get; private set; }

        /// <summary>
        /// Gets current paged result page size
        /// </summary>
        [JsonProperty(PropertyName = "pageSize")]
        public int? PageSize { get; private set; }

        /// <summary>
        /// Gets total number of paged result pages for given resource
        /// </summary>
        [JsonProperty(PropertyName = "totalPages")]
        public int? TotalPages { get; private set; }

        /// <summary>
        /// Gets current paged result page of results
        /// </summary>
        [JsonProperty(PropertyName = "results")]
        public IList<T> Results { get; private set; }
    }
}
