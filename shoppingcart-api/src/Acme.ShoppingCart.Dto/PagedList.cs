using System;
using System.Collections.Generic;

namespace Acme.ShoppingCart.Dto {
    public class PagedList<T> {
        public int TotalItems { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public IList<T> Items { get; set; }

        public PagedList<TOutput> Convert<TOutput>(Func<T, TOutput> converter) {
            var result = new PagedList<TOutput> {
                TotalItems = TotalItems,
                PageNumber = PageNumber,
                PageSize = PageSize,
                Items = new List<TOutput>()
            };

            foreach (var item in Items) {
                result.Items.Add(converter(item));
            }

            return result;
        }
    }
}
