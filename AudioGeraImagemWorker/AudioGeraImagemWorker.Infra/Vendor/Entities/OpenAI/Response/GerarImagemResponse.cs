using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioGeraImagemWorker.Infra.Vendor.Entities.OpenAI.Response
{
    public class GerarImagemResponse
    {
        public int created { get; set; }
        public List<Datum> data { get; set; }
    }

    public class Datum
    {
        public string url { get; set; }
    }
}
