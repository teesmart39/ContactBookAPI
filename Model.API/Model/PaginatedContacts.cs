using Model.API.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.API.Model
{
    public class PaginatedContacts
    {
        public int CurrentPage { get; set; }
        public int PerPageSize { get; set; }
        public int TotalPages { get; set; }
        public IEnumerable<Contacts> Contacts { get; set; }
    }
}
