using Acme.ShoppingCart.Dto;
using Acme.ShoppingCart.WebApi.Models;

namespace Acme.ShoppingCart.WebApi.Mappers {
    public class SubjectModelMapper {
        public SubjectModel Map(SubjectDto dto) {
            if (dto == null) {
                return null;
            }

            return new SubjectModel {
                SubjectId = dto.SubjectId,
                Name = dto.Name,
                GivenName = dto.GivenName,
                FamilyName = dto.FamilyName,
                UserPrincipalName = dto.UserPrincipalName
            };
        }
    }
}
