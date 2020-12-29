using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Domain
{
    public class IdentifyAssetsMessage
    {
        public string Scope { get; set; }
        public Uri Url { get; set; }
    }
}