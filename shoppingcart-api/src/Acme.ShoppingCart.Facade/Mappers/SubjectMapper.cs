using Acme.ShoppingCart.Domain.Entities;
using Acme.ShoppingCart.Dto;

namespace Acme.ShoppingCart.Facade.Mappers {
    public class SubjectMapper {
        public SubjectDto? MapToDto(Subject entity) {
            if (entity == null) {
                return null;
            }

            return new SubjectDto() {
                SubjectId = entity.SubjectId,
                GivenName = entity.GivenName,
                FamilyName = entity.FamilyName,
                Name = entity.Name,
                UserPrincipalName = entity.UserPrincipalName
            };
        }
    }
}
