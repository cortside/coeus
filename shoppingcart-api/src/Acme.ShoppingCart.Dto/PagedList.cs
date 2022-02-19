using System.Collections.Generic;

namespace Acme.ShoppingCart.Dto {
    public class PagedList<T> {
        public int TotalItems { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public IList<T> Items { get; set; }
    }
}
