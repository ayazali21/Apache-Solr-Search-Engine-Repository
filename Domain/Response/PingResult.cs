using System;
using System.Collections.Generic;
using System.Text;

namespace SolrLib.Domain.Response
{
    public class PingResult
    {
        public int Status { get; set; }
        public int QTime { get; set; }
        public IDictionary<string, string> Params { get; set; }

    }
}
