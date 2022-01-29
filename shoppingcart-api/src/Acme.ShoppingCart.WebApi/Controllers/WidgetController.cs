using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Acme.ShoppingCart.DomainService;
using Acme.ShoppingCart.Dto;
using Acme.ShoppingCart.WebApi.Models.Requests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Serilog.Context;

namespace Acme.ShoppingCart.WebApi.Controllers {
    /// <summary>
    /// Represents the shared functionality/resources of the widget resource
    /// </summary>
    [ApiVersion("1")]
    [Produces("application/json")]
    [ApiController]
    [Route("api/v1/widgets")]
    public class WidgetController : Controller {
        private readonly ILogger logger;
        private readonly IWidgetService service;

        /// <summary>
        /// Initializes a new instance of the WidgetController
        /// </summary>
        public WidgetController(ILogger<WidgetController> logger, IWidgetService service) {
            this.logger = logger;
            this.service = service;
        }

        /// <summary>
        /// Gets widgets
        /// </summary>
        [HttpGet("")]
        //[Authorize(Constants.Authorization.Permissions.GetWidgets)]
        [ProducesResponseType(typeof(List<WidgetDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetWidgetsAsync() {
            var widgets = await service.GetWidgetsAsync().ConfigureAwait(false);
            return Ok(widgets);
        }

        /// <summary>
        /// Gets a widget by id
        /// </summary>
        /// <param name="id">the id of the widget to get</param>
        [HttpGet("{id}")]
        [ActionName(nameof(GetWidgetAsync))]
        //[Authorize(Constants.Authorization.Permissions.GetWidget)]
        [ProducesResponseType(typeof(WidgetDto), 200)]
        public async Task<IActionResult> GetWidgetAsync(int id) {
            var widget = await service.GetWidgetAsync(id).ConfigureAwait(false);
            return Ok(widget);
        }

        /// <summary>
        /// Create a new widget
        /// </summary>
        /// <param name="input"></param>
        [HttpPost("")]
        //[Authorize(Constants.Authorization.Permissions.CreateWidget)]
        [ProducesResponseType(typeof(WidgetDto), 201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateWidgetAsync([FromBody] WidgetRequest input) {
            var dto = new WidgetDto() {
                Text = input.Text,
                Width = input.Width,
                Height = input.Height
            };
            var widget = await service.CreateWidgetAsync(dto).ConfigureAwait(false);
            return CreatedAtAction(nameof(GetWidgetAsync), new { id = widget.WidgetId }, widget);
        }

        /// <summary>
        /// Update a widget
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        [HttpPut("{id}")]
        //[Authorize(Constants.Authorization.Permissions.UpdateWidget)]
        [ProducesResponseType(typeof(WidgetDto), 204)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> UpdateWidgetAsync(int id, WidgetRequest input) {
            using (LogContext.PushProperty("WidgetId", id)) {
                var dto = new WidgetDto() {
                    WidgetId = id,
                    Text = input.Text,
                    Width = input.Width,
                    Height = input.Height
                };

                var widget = await service.UpdateWidgetAsync(dto).ConfigureAwait(false);
                return StatusCode((int)HttpStatusCode.NoContent, widget);
            }
        }

        /// <summary>
        /// Update a widget
        /// </summary>
        /// <param name="id"></param>
        [HttpPost("{id}/publish")]
        //[Authorize(Constants.Authorization.Permissions.UpdateWidget)]
        [ProducesResponseType(typeof(WidgetDto), 204)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> PublishWidgetStateChangedEventAsync(int id) {
            using (LogContext.PushProperty("WidgetId", id)) {
                await service.PublishWidgetStateChangedEventAsync(id).ConfigureAwait(false);
                return StatusCode((int)HttpStatusCode.NoContent);
            }
        }
    }
}
