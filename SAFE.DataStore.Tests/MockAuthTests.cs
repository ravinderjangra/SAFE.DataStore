using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SAFE.DataStore.Client.Auth;

namespace SAFE.DataStore.Tests
{
    [TestClass]
    public class MockAuthTests : TestBase
    {
        [TestMethod]
        public async Task RandomCredentialsResultsInASession()
        {
            try
            {
                var session = await GetAuth().MockAuthenticationAsync(Credentials.Random);
                Assert.IsTrue(session.HasValue);
                Assert.IsNotNull(session.Value);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod]
        public async Task CanLoginAfterCreatingAccount()
        {
            try
            {
                var auth = GetAuth();
                var credentials = Credentials.Random;
                var sessionResult_1 = await auth.MockAuthenticationAsync(credentials);
                var session_1 = sessionResult_1.Value;

                var sessionResult_2 = await auth.MockAuthenticationAsync(credentials);
                var session_2 = sessionResult_2.Value;

                Assert.IsTrue(sessionResult_2.HasValue);
                Assert.IsNotNull(sessionResult_2.Value);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }
    }
}
