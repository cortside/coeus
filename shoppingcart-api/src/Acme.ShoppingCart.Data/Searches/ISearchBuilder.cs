// use version from cortside.aspnetcore.entityframework

using System.Linq;

namespace Acme.ShoppingCart.Data.Searches {
    public interface ISearchBuilder<T> {
        IQueryable<T> Build(IQueryable<T> entities);
    }
}
