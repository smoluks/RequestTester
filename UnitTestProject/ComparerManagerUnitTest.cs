using Microsoft.VisualStudio.TestTools.UnitTesting;
using RequestCore.Enums;
using RequestTester.Entities;
using RequestTester.Managers;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace UnitTestProject
{
    [TestClass]
    public class ComparerManagerUnitTest
    {
        [TestMethod]
        public async Task TestMethod1Async()
        {
            var testCase = new RequestCase(
                    new Request
                    {
                        requestType = RequestType.GET,
                        path = @"/search",
                        parameters = new Dictionary<string, string>() { { "q", "42" } }
                    }
                );

            await RequestParallelManager.SendRequestsParallelAsync(
                testCase,
                new string[] { "https://www.google.com", "http://www.google.com" },
                CancellationToken.None);

            Assert.AreEqual(testCase._status, CaseStatus.Equals);
        }
    }
}
