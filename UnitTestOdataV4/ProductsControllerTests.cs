using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using OdataAPI;
using OdataAPI.Controllers;
using OdataAPI.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using UnitTestOdataV4.Helper;

namespace UnitTestOdataV4
{

    [TestClass]
    public class ProductsControllerTests  
    {
        private readonly HttpClient client;

        public ProductsControllerTests()
        {
            var webHostBuilder = new WebHostBuilder()
                .UseEnvironment("Development")
                .UseStartup<Startup>();
            var server = new TestServer(webHostBuilder);
            client = server.CreateClient();
        }

        


        [DataTestMethod]
        [DataRow(1)]
        [DataRow(2)]
        [DataRow(3)]
        [DataRow(4)]
        public async Task Products_Single_Ok_Result(int key)
        {
            bool result = false;

                // Act
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get,"/odata/Products("+key+")");

            HttpResponseMessage response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode){
                var responseString = response.Content.ReadAsStringAsync().Result;
                if (!string.IsNullOrEmpty(responseString))
                {
                    var _result = JsonConvert.DeserializeObject<Models.SingleProductOdataModel>(responseString);
                    if (_result != null )
                    {
                        result = true;
                    }
                    else { result = false; }
                }
                else { result = false; }
            }
            else {
                result = false;
            }
            // Assert
            Assert.IsTrue(result, $"TRUE iD: {key} status code:" + response.StatusCode);
        }

        [DataTestMethod]
        [DataRow(5)]
        [DataRow(6)]
        [DataRow(7)]
        [DataRow(8)]
        public async Task Products_Single_False_Result(int key)
        {
            bool result = false;

            // Act
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "/odata/Products(" + key + ")");

            HttpResponseMessage response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var responseString = response.Content.ReadAsStringAsync().Result;
                if (!string.IsNullOrEmpty(responseString))
                {
                    var _result = JsonConvert.DeserializeObject<Models.SingleProductOdataModel>(responseString);
                    if (_result != null)
                    {
                        result = true;
                    }
                    else { result = false; }
                }
                else { result = false; }
            }
            else
            {
                result = false;
            }
            // Assert
            Assert.IsFalse(result, $"False iD: {key} status code:" + response.StatusCode);
        }

        [TestMethod]
        public async Task All_Produkts()
        {
            bool _result = false;
            int count = 0;
            // Act
            HttpRequestMessage request = new HttpRequestMessage(
                HttpMethod.Get, "/odata/Products");

            HttpResponseMessage response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var responseString = response.Content.ReadAsStringAsync().Result;
                var objResult = JsonConvert.DeserializeObject<Models.OdataModel<Product>>(responseString);

                count = objResult.Value.Count();
                if (count != 4) { _result = false; } else { _result = true; }
            }
            else
            {
                _result = false;
            }

            Assert.IsTrue(_result, $"All test products should be 4 but it is: [ " + count + " ]");
        }


    }
}
