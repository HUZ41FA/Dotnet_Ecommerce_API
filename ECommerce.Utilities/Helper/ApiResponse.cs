using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Utilities.Helper
{
    public class ApiResponse
    {
        public bool Status { get; set; }
        public List<string> Messages { get; set; }
        public object Data { get; set; }
    }
}
