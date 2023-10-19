using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Acme.IdentityServer.Data;
using Acme.IdentityServer.WebApi.Data;
using Acme.IdentityServer.WebApi.Models;

namespace Acme.IdentityServer.WebApi.Assemblers.Implementors.Profiles {
    public class UserModelProfile : Profile {
        public UserModelProfile() {
            CreateMap<User, Models.Output.UserOutputModel>()
                .ForMember(dest => dest.Username,
                    (IMemberConfigurationExpression<User, Models.Output.UserOutputModel, string> x) =>
                        x.MapFrom(src => src.Username))

                .ForMember(dest => dest.ProviderName,
                    (IMemberConfigurationExpression<User, Models.Output.UserOutputModel, string> x) =>
                        x.MapFrom(src => src.ProviderName))

                .ForMember(dest => dest.ProviderSubjectId,
                    (IMemberConfigurationExpression<User, Models.Output.UserOutputModel, string> x) =>
                        x.MapFrom(src => src.ProviderSubjectId))

                .ForMember(dest => dest.IsActive,
                    (IMemberConfigurationExpression<User, Models.Output.UserOutputModel, bool> x) =>
                        x.MapFrom(src => src.UserStatus.Equals(IdsDefinitions.Active)))

                .ForMember(dest => dest.LastModifiedBySubjectId,
                    (IMemberConfigurationExpression<User, Models.Output.UserOutputModel, Guid> x) =>
                        x.MapFrom(src => src.LastModifiedUserId.ToString()))

                .ForMember(dest => dest.LockedReason,
                    (IMemberConfigurationExpression<User, Models.Output.UserOutputModel, string> x) =>
                        x.MapFrom(src => src.LockedReason))

                .ForMember(dest => dest.Claims, opt => opt.MapFrom((src, dest) => {
                    if (src.UserClaims == null || src.UserClaims.Count == 0) {
                        return null;
                    }

                    List<UserClaimModel> userClaims = new List<UserClaimModel>();
                    foreach (var item in src.UserClaims) {
                        userClaims.Add(new UserClaimModel() {
                            Type = item.Type,
                            Value = item.Value
                        });
                    }
                    return userClaims;
                }));

            CreateMap<Client, Models.Output.UserOutputModel>()
                .ForMember(dest => dest.Username,
                    (IMemberConfigurationExpression<Client, Models.Output.UserOutputModel, string> x) =>
                        x.MapFrom(src => src.ClientId))

                .ForMember(dest => dest.UserId,
                    (IMemberConfigurationExpression<Client, Models.Output.UserOutputModel, Guid> x) =>
                        x.MapFrom(src => src.ClientClaims.FirstOrDefault(c => c.Type == "sub").Value))

                .ForMember(dest => dest.IsActive,
                    (IMemberConfigurationExpression<Client, Models.Output.UserOutputModel, bool> x) =>
                        x.MapFrom(src => src.Enabled))

                .ForMember(dest => dest.Claims, opt => opt.MapFrom((src, dest) => {
                    if (src.ClientClaims == null || src.ClientClaims.Count == 0) {
                        return null;
                    }

                    List<UserClaimModel> userClaims = new List<UserClaimModel>();
                    foreach (var item in src.ClientClaims) {
                        userClaims.Add(new UserClaimModel() {
                            Type = item.Type,
                            Value = item.Value
                        });
                    }
                    return userClaims;
                }));
        }
    }
}
