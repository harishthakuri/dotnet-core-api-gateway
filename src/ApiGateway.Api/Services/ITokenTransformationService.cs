using System.Security.Claims;

namespace ApiGateway.Api.Services;

public interface ITokenTransformationService
{
    string GenerateInternalToken(ClaimsPrincipal externalUser);
}