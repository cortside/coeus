using System.Threading.Tasks;
using Acme.ShoppingCart.Dto;

namespace Acme.ShoppingCart.DomainService {
    public interface ISubjectService {
        Task SaveAsync(SubjectDto subject);
    }
}
