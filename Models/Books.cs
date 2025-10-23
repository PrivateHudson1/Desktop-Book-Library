using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Models
{
    public class Books
    {
        public int id { get; set; }
        public string fileName { get; set; }
        public int authorId { get; set; }
        public int genreId { get; set; }
        public int publicationYear { get; set; }
        public string isbn { get; set; }
        public string fileType { get; set; }
        public int? idFileContent { get; set; }
        public byte[] image { get; set; }

        public string description { get; set; }
        public string rating { get; set; }
        public int? countPages { get; set; }

    }
}
