namespace Acme.ShoppingCart.WebApi.Models.Responses {
    /// <summary>
    /// Represents a single customer
    /// </summary>
    public class CustomerTypeModel {
        public int CustomerTypeId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool TaxExempt { get; set; }
    }
}
