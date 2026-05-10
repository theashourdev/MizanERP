using MizanERP.Domain.Entities;

namespace MizanERP.Application.Interfaces;

public interface IJwtService
{
    string GenerateAccessToken(ApplicationUser user, IList<string> roles);
    string? GetUserIdFromExpiredToken(string accessToken);  // used in refresh flow
}