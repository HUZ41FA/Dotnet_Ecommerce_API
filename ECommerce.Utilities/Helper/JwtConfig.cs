using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Utilities.Helper
{
    public class JwtConfig
    {
        public string ValidAudience { get; set; }
        public string ValidHost { get; set; }
        public string Secret { get; set; }
    }
}
