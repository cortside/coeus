namespace Acme.ShoppingCart.Dto {
    public class SearchDto {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string Sort { get; set; }
    }
}
