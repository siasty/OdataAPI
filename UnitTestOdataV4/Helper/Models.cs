using Newtonsoft.Json;
using OdataAPI.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestOdataV4.Helper
{
    public class Models
    {
        public class OdataModel<T>
        {
            [JsonProperty("@odata.context")]
            public string OdataContext { get; set; }
            [JsonProperty("value")]
            public IEnumerable<T> Value { get; set; }
        }
        public class SingleProductOdataModel
        {
            [JsonProperty("@odata.context")]
            public string OdataContext { get; set; }
            public int Id { get; set; }
            public string Name { get; set; }
            public decimal Price { get; set; }
            public string Category { get; set; }
            public int? SupplierId { get; set; }
        }

    }
}
