using System.Linq;

namespace Acme.ShoppingCart.Data.Searches {
    public interface ISearchBuilder<T> {
        IQueryable<T> Build(IQueryable<T> list);
    }
}
