using Microsoft.VisualStudio.TestTools.UnitTesting;
using RequestTester.Entities;
using RequestTester.Managers;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace UnitTestProject
{
    [TestClass]
    public class RequestManagerUnitTest
    {
        [TestMethod]
        public async Task TestMethod1()
        {
            var result = await RequestManager.MakeRequest(new Request
            {
                requestType = Request.RequestType.GET,
                parameters = new Dictionary<string, string>(){ { "text", "42"} }
            },
            "https://ya.ru/",
            CancellationToken.None);

            Assert.IsTrue(result.isSuccessed);
            Assert.AreEqual(result.statusCode, System.Net.HttpStatusCode.OK);
            Assert.IsTrue(result.data.Contains(@"<!DOCTYPE html>"));
        }
    }
}
