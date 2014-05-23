using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TridionCoreServiceAPI.Models
{
    public class TridionItem
    {
        public string Title { get; set; }
        public string Uri { get; set; }
        public string LastModifiedBy { get; set; }
        public string Error { get; set; }
        public bool IsPublished { get; set; }
    }
}