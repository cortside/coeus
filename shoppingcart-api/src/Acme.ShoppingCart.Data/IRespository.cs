namespace Acme.ShoppingCart.Data {
    public interface IRepository<T> { //where T : IAggregateRoot {
        IUnitOfWork UnitOfWork { get; }
    }
}
