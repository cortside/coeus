using System.Linq;
using System.Threading.Tasks;
using Acme.ShoppingCart.Facade;
using Acme.ShoppingCart.WebApi.Mappers;
using Acme.ShoppingCart.WebApi.Models.Responses;
using Cortside.AspNetCore.Common.Models;
using Cortside.AspNetCore.Common.Paging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Acme.ShoppingCart.WebApi.Controllers {
    /// <summary>
    /// Represents the shared functionality/resources of the customer resource
    /// </summary>
    [ApiVersion("1")]
    [ApiVersion("2")]
    [Produces("application/json")]
    [ApiController]
    [Route("api/v{version:apiVersion}/customertypes")]
    public class CustomerTypeController : Controller {
        private readonly ICustomerTypeFacade facade;
        private readonly CustomerTypeModelMapper customerMapper;

        /// <summary>
        /// Initializes a new instance of the CustomerController
        /// </summary>
        public CustomerTypeController(ICustomerTypeFacade facade, CustomerTypeModelMapper customerMapper) {
            this.facade = facade;
            this.customerMapper = customerMapper;
        }

        /// <summary>
        /// Gets customers
        /// </summary>
        [HttpGet("")]
        [ProducesResponseType(typeof(PagedList<CustomerTypeModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCustomerTypesAsync() {
            var results = await facade.GetCustomerTypesAsync().ConfigureAwait(false);
            var models = results.Results.Select(x => customerMapper.Map(x)).OrderBy(x => x.CustomerTypeId).ToList();
            return Ok(new ListResult<CustomerTypeModel>(models));
        }
    }
}
