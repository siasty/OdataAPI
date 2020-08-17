using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OdataAPI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestOdataV4
{
    [TestClass]
    public class ODataTests
    {
        private readonly HttpClient client;

        public ODataTests()
        {
            var webHostBuilder = new WebHostBuilder()
                .UseEnvironment("Development")
                .UseStartup<Startup>();
            var server = new TestServer(webHostBuilder);
            client = server.CreateClient();
        }

        [TestMethod]
        public async Task Get_Metadata()
        {
            // Act
            HttpRequestMessage request = new HttpRequestMessage(
                HttpMethod.Get, "/odata/$metadata");

            HttpResponseMessage response = await client.SendAsync(request);

            Assert.IsTrue(response.IsSuccessStatusCode, $"Status code: " + response.StatusCode.ToString());
        }
    }
}
