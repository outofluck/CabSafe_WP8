using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CabSafe.DataModel
{

    public class TaxiInformation
    {
        public int id { get; set; }
        public string SenderName { get; set; }
        public string PlateNum { get; set; }
        public string DriverName { get; set; }
        public string OtherMessage { get; set; }
        public string referenceCode { get; set; }
        public string Longitude { get; set; }
        public string Latitude { get; set; }
    }

}