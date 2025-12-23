using Backend.Data;
using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services
{
    public class AuthHelpers
    {
        public static async Task<User?> GetUserByBearerTokenAsync(UtilitiesDbContext context, HttpRequest request)
        {
            var authHeader = request.Headers["Authorization"].ToString();
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                return null;

            var token = authHeader.Substring("Bearer ".Length);
            var authToken = await context.AuthTokens
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.Token == token);

            return authToken?.User;
        }
    }
}
