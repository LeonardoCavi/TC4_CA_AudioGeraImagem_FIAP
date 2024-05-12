using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioGeraImagemWorker.Infra.Vendor.Entities.AzureBlob
{
    [ExcludeFromCodeCoverage]
    public class AzureBlobParameters
    {
        public string ConnectionString { get; set; }
        public string ContainerName { get; set; }
    }
}
