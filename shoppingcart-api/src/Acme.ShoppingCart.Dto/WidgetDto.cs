namespace Acme.ShoppingCart.Dto {
    public class WidgetDto : AuditableEntityDto {
        public int WidgetId { get; set; }
        public string Text { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }
}
