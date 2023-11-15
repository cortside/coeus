using Acme.IdentityServer.WebApi.Services;
using Xunit;

namespace Acme.IdentityServer.WebApi.Tests.Services {

    public class HashProviderTest {
        [Fact]
        public void ShouldBeAbleToHashString() {
            //Arrange 
            var target = new HashProvider();
            var theString = "test";

            //Act
            var result = target.ComputeHash(theString);

            //Assert
            Assert.Equal("7iaw3Ur350mqGo7jwQrpkj9hiYB3Lkc/iBml1JQODbJ6wYX4oOHV+E+IvIh/1nsUNzLDBMxfqa2Ob1f1ACio/w==", result);
        }

        [Fact]
        public void ShouldGenerateSalt() {
            // act
            var target = new HashProvider();
            var result = target.GenerateSalt();

            // assert
            Assert.NotEmpty(result);
        }
    }
}
