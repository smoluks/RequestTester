using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RequestTester.Entities;
using RequestTester.Managers;

namespace UnitTestProject
{
    [TestClass]
    public class ComparerManagerUnitTest
    {
        [TestMethod]
        public async Task TestMethod1Async()
        {
            var result = await ComparerManager.Compare(
                new Request
                {
                    requestType = Request.RequestType.GET,
                    parameters = new Dictionary<string, string>() { { "text", "42" } }
                },
                new string[] { "https://ya.ru/", "https://yandex.ru/" },
                CancellationToken.None);

            Assert.IsTrue(result.result  == ComparerManager.CompareResult.Equals);
        }
    }
}
