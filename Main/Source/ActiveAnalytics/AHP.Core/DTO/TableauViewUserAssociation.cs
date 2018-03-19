using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHP.Core.DTO
{
    public class TableauViewUserAssociation
    {
        public string UserType { get; set; }

        public string Username { get; set; }

        public string ViewId { get; set; }

        public bool Selected { get; set; }    
    }
}
