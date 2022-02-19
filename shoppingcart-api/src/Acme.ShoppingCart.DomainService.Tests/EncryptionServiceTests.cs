//using System;
//using System.Collections.Generic;
//using Acme.ShoppingCart.Configuration;
//using Acme.ShoppingCart.Dto;
//using Acme.ShoppingCart.Dto.Enumerations;
//using FluentAssertions;
//using Xunit;

//namespace Acme.ShoppingCart.DomainService.Tests {
//    public class EncryptionServiceTests {
//        private readonly EncryptionConfiguration encryptionConfiguration;
//        private readonly IEncryptionService encryptionService;

//        public EncryptionServiceTests() {
//            encryptionConfiguration = new EncryptionConfiguration {
//                AesIv = "loinYTfzpE8S4FblzcwV2Q==",
//                AesKey = "PVGkk3lWAVd+awhCeI0o9DIw/DV5PGZM3i2S9CFa8Io="
//            };
//            encryptionService = new EncryptionService(encryptionConfiguration);
//        }

//        [Fact]
//        public void ShouldEncryptAndDecryptSearchObject() {
//            // arrange
//            RebateSearchDto rebateSearchDto = new RebateSearchDto {
//                ContractorIds = new List<int> { 1 },
//                LoanId = Guid.NewGuid(),
//                RebateStatus = RebateRequestStatus.Created
//            };

//            // act
//            string response = encryptionService.EncryptString(rebateSearchDto);

//            // assert
//            response.Should().NotBeNullOrWhiteSpace();

//            // act
//            RebateSearchDto rebateSearchDtoDecrypted = encryptionService.DecryptString<RebateSearchDto>(response);

//            // assert
//            rebateSearchDtoDecrypted.Should().NotBeNull();
//            rebateSearchDtoDecrypted.ContractorIds.Should().BeEquivalentTo(rebateSearchDto.ContractorIds);
//            rebateSearchDtoDecrypted.LoanId.Should().Be(rebateSearchDto.LoanId);
//            rebateSearchDtoDecrypted.RebateStatus.Should().Be(rebateSearchDto.RebateStatus);
//        }
//    }
//}
