using System.ComponentModel.DataAnnotations;

namespace EnerBank.IdentityServer.WebApi.Models.Input {
    public class UpdatePasswordModel {
        /// <summary>
        /// New password
        /// </summary>
        [Required]
        public string Password { get; set; }
    }
}
