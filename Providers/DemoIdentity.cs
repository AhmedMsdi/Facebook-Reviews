using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Atreemo
{
    public class DemoIdentity : ClaimsIdentity
    {
        private static string SERVICE_URI = HttpContext.Current.Request.Url.AbsoluteUri.Substring(0, HttpContext.Current.Request.Url.AbsoluteUri.IndexOf("//") + 2) + HttpContext.Current.Request.Url.Authority
            + (System.Configuration.ConfigurationManager.AppSettings["SubDomainName"] != null ? "/" + System.Configuration.ConfigurationManager.AppSettings["SubDomainName"] : "");
        public string RolesClaimType = SERVICE_URI+ "Security.Role";
        public string GroupClaimType = SERVICE_URI + "Security.Group";
        public string IPClaimType = SERVICE_URI + "Security.IP";
        public string IdClaimType = SERVICE_URI + "Security.Id";


        public DemoIdentity(IEnumerable<Claim> claims, string authenticationType)
            : base(claims, authenticationType: authenticationType)
        {

            AddClaims(from @group in claims where @group.Type == GroupClaimType select @group);
            AddClaims(from role in claims where role.Type == RoleClaimType select role);
            AddClaims(from id in claims where id.Type == IdClaimType select id);
            AddClaims(from ip in claims where ip.Type == IPClaimType select ip);


        }

        public DemoIdentity(IEnumerable<string> roles, IEnumerable<string> groups, string IP, int id)
        {
            AddClaims(from @group in groups select new Claim(GroupClaimType, @group));
            AddClaims(from role in roles select new Claim(RolesClaimType, role));
            AddClaim(new Claim(IdClaimType, id.ToString()));
            AddClaim(new Claim(IPClaimType, IP.ToString()));
        }

        public IEnumerable<string> Roles
        {
            get
            {
                return from claim in FindAll(RolesClaimType) select claim.Value;
            }
        }

        public IEnumerable<string> Groups { get { return from claim in FindAll(GroupClaimType) select claim.Value; } }

        public int Id { get { return Convert.ToInt32(FindFirst(IdClaimType).Value); } }
        public string IP { get { return FindFirst(IPClaimType).Value; } }
    }
    
}
