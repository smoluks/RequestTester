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
    public class RequestManagerUnitTest
    {
        [TestMethod]
        public async Task TestMethod1()
        {
            var responce = await RequestManager.SendRequestAsync(new Request
            {
                requestType = RequestType.GET,
                parameters = new Dictionary<string, string>() { { "text", "42" } }
            },
            "https://ya.ru/",
            CancellationToken.None);

            Assert.IsTrue(responce.isSuccessed);
            Assert.AreEqual(responce.statusCode, System.Net.HttpStatusCode.OK);
            Assert.IsTrue(responce.body.Contains(@"<!DOCTYPE html>"));
        }
    }
}
