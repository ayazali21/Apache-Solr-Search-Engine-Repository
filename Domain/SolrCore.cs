using SolrNet.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace SolrLib.Domain
{
    public abstract class SolrCore
    {

        [SolrUniqueKey("id")]
        public string Id { get; set; }
        //public abstract string GetCoreName();
    }
}
