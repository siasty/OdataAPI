using Microsoft.AspNet.OData.Batch;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OdataAPI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
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


        [TestMethod]
        public async Task Get_SimpleBatchRequest()
        {
            // Act
            HttpRequestMessage request = new HttpRequestMessage(
                HttpMethod.Post, "/odata/$batch");


            string batchName = $"batch_{Guid.NewGuid()}";
            var batchContent = new MultipartContent("mixed", batchName);

            var requestMessage = new HttpRequestMessage(HttpMethod.Get, "http://localhost:5000/odata/Products?$orderby=Name&$select=Id,Name&$skip=0&$top=20");
            var messageContent = new HttpMessageContent(requestMessage);

            if (messageContent.Headers.Contains("Content-Type"))
            {
                messageContent.Headers.Remove("Content-Type");
            }
            messageContent.Headers.Add("Content-Type", "application/http");
            messageContent.Headers.Add("Content-Transfer-Encoding", "binary");
            requestMessage.Headers.Add("Accept", "application/json");

            batchContent.Add(messageContent);

            request.Content = batchContent;

            HttpResponseMessage response = await client.SendAsync(request);

            var responseString = response.Content.ReadAsStringAsync().Result;
            Console.WriteLine(responseString);
            Assert.IsTrue(response.IsSuccessStatusCode, $"Status code: " + response.StatusCode.ToString());
        }

        [TestMethod]
        public async Task Check_firstBoundryBatchRequest_has_CRLR()
        {
            // Act
            HttpRequestMessage request = new HttpRequestMessage(
                HttpMethod.Post, "/odata/$batch");


            string batchName = $"batch_{Guid.NewGuid()}";
            var batchContent = new MultipartContent("mixed", batchName);

            var requestMessage = new HttpRequestMessage(HttpMethod.Get, "http://localhost:5000/odata/Products?$orderby=Name&$select=Id,Name&$skip=0&$top=20");
            var messageContent = new HttpMessageContent(requestMessage);

            if (messageContent.Headers.Contains("Content-Type"))
            {
                messageContent.Headers.Remove("Content-Type");
            }
            messageContent.Headers.Add("Content-Type", "application/http");
            messageContent.Headers.Add("Content-Transfer-Encoding", "binary");
            requestMessage.Headers.Add("Accept", "application/json");

            batchContent.Add(messageContent);

            request.Content = batchContent;

            HttpResponseMessage response = await client.SendAsync(request);

            var responseString = response.Content.ReadAsStringAsync().Result;
            Debug.WriteLine(responseString);

            var test = responseString.Take(54).ToArray();
            Console.WriteLine(test);
            Console.WriteLine("Lenght: "+test.Length);
            Console.WriteLine("last character: " + test[51]);
            Console.WriteLine("char52: " + (int)test[52]);
            Console.WriteLine("char53: " + (int)test[53]);

            Assert.IsTrue(response.IsSuccessStatusCode, $"Status code: " + response.StatusCode.ToString());
        }
    }
}
