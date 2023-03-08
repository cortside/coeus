using System.Threading.Tasks;
using Acme.ShoppingCart.CatalogApi.Models.Responses;

namespace Acme.ShoppingCart.CatalogApi {
    public interface ICatalogClient {
        Task<CatalogItem> GetItemAsync(string sku);
    }
}
