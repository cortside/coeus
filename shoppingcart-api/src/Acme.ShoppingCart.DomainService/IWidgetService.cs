using System.Collections.Generic;
using System.Threading.Tasks;
using Acme.ShoppingCart.Dto;

namespace Acme.ShoppingCart.DomainService {
    public interface IWidgetService {
        Task<WidgetDto> CreateWidgetAsync(WidgetDto dto);
        Task<WidgetDto> GetWidgetAsync(int widgetId);
        Task<List<WidgetDto>> GetWidgetsAsync();
        Task<WidgetDto> UpdateWidgetAsync(WidgetDto dto);
        Task<WidgetDto> DeleteWidgetAsync(int widgetId);
        Task PublishWidgetStateChangedEventAsync(int id);
    }
}
