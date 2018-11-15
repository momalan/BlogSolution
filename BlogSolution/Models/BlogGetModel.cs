using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BlogSolution.Models
{
    public class BlogGetModel
    {
        public string slug { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string body { get; set; }
        public Nullable<System.DateTime> createdAt { get; set; }
        public Nullable<System.DateTime> updatedAt { get; set; }
        public List<string> tagList { get; set; }
    }
}