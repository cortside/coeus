using System;
using System.ComponentModel.DataAnnotations;

namespace Acme.ShoppingCart.WebApi.Models.Responses {
    /// <summary>
    /// Subject
    /// </summary>
    public class SubjectModel {
        /// <summary>
        /// SubjectId
        /// </summary>
        public Guid SubjectId { get; set; }

        /// <summary>
        /// Name
        /// </summary>
        [StringLength(100)]
        public string Name { get; set; }

        /// <summary>
        /// GivenName
        /// </summary>
        [StringLength(100)]
        public string GivenName { get; set; }

        /// <summary>
        /// FamilyName
        /// </summary>
        [StringLength(100)]
        public string FamilyName { get; set; }

        /// <summary>
        /// UserPrincipalName
        /// </summary>
        [StringLength(100)]
        public string UserPrincipalName { get; set; }

        /// <summary>
        /// CreatedDate
        /// </summary>
        public DateTime CreatedDate { get; set; }
    }
}
