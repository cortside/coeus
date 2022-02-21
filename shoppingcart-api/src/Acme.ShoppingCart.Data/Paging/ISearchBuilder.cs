using System.Linq;

namespace Acme.ShoppingCart.Data.Paging {
    public interface ISearchBuilder<T> {
        IQueryable<T> Build(IQueryable<T> list);
    }
}
