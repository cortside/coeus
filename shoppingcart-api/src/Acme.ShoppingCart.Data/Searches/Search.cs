namespace Acme.ShoppingCart.Data.Searches {
    public class Search : ISearch {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string Sort { get; set; }
    }
}
