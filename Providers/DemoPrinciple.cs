using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Atreemo
{
    public class DemoPrincipal : ClaimsPrincipal
    {


        public DemoPrincipal(DemoIdentity identity)
            : base(identity)
        {

        }

        public DemoPrincipal(ClaimsPrincipal claimsPrincipal)
            : base(claimsPrincipal)
        {

        }
    }
}
