using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Models
{
    public class Users
    {
        public int id { get; set; }
        public string firstName { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public int isActive { get; set; }
     
    }
}
