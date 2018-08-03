using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Models
{
    public class Notes
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public bool IsPlainText { get; set; }
        public List<Content> Content { get; set; }
        public List<Label> Label { get; set; }
        public bool IsPinned { get; set; }
    }
}
