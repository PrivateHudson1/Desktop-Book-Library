using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Models
{
    public class UpdateObjectModel
    {
        public int id { get; set; }
        public string fileName { get; set; }
        public byte[] image { get; set; }
        public int idFile { get; set; }
        public string Author { get; set; }
        public string rating { get; set; }
        public int countPages { get; set; }
        public string description { get; set; }
        public int genreId { get; set; }
        public bool isEnable { get; set; }
        public string isContent { get; set; }
        public bool isBackground { get; set; }
    }
}
