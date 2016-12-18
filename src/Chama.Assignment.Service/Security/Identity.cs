using System.Security.Claims;
using System.Security.Principal;

namespace Chama.Assignment.Service.Security
{
    public class Identity : IIdentity
    {
        private readonly ClaimsIdentity _claimsIdentity;

        public Identity(ClaimsIdentity claimsIdentity)
        {
            _claimsIdentity = claimsIdentity;
        }

        public string Name
        {
            get { return _claimsIdentity.Name; }
        }

        public string AuthenticationType
        {
            get { return "Basic"; }
        }

        public bool IsAuthenticated
        {
            get { return true; }
        }
    }
}
