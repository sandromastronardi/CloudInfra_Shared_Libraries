using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
#if(NETCOREAPP || NETSTANDARD)
using Microsoft.AspNetCore.Http;
#endif
namespace CompanyName.Shared.Authentication
{
    public interface IUserAuthenticationService
    {
        Task<Identity> GetUserAsync(HttpRequestMessage req);
        Task<Identity> GetUserAsync(HttpRequest req);
        Identity Identity { get; set; }
    }
}
