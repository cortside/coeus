using Acme.IdentityServer.WebApi.Helpers;
using Xunit;

namespace Acme.IdentityServer.WebApi.Tests.Helpers {
    public class PhoneNumberHelperTests {
        private readonly PhoneNumberHelper sut;

        public PhoneNumberHelperTests() {
            sut = new PhoneNumberHelper();
        }

        [Theory]
        [InlineData("(123)-456-7890")]
        [InlineData("(123)456-7890")]
        [InlineData("(123) 456-7890")]
        [InlineData("(123) 456 7890")]
        [InlineData("123-456-7890")]
        [InlineData("123 456 7890")]
        public void FormatPhoneNumber_ShouldFormatCorrectly(string phoneNumber) {
            // Arrange

            // Act
            var result = sut.FormatPhoneNumber(phoneNumber);

            // Assert
            Assert.Equal("1234567890", result);
        }
    }
}
