using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
namespace Backend.Controllers
{
    public class BaseController : ControllerBase
    {
        protected int GetUserIdFromToken() { 
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value; if (string.IsNullOrEmpty(userIdClaim)) 
                throw new Exception("UserID not found in token"); 
            return int.Parse(userIdClaim); 
        }
    }
}
