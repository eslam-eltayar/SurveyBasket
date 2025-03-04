using Microsoft.Extensions.Options;
using SurveyBasket.Authentication;

namespace SurveyBasket.Controllers;

[Route("[controller]")]
[ApiController]
public class AuthController(IAuthService authService, IOptions<JwtOptions> jwtOptions) : ControllerBase
{
    private readonly IAuthService _authService = authService;
    private readonly JwtOptions _jwtOptions = jwtOptions.Value;

    [HttpPost("")]
    public async Task<IActionResult> LoginAsync([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var authResult = await _authService.GetTokenAsync(request.Email, request.Password, cancellationToken);

        return authResult is null ? BadRequest("Invalid email/password") : Ok(authResult);
    }

    [HttpPost("register")]
    public async Task<IActionResult> RegisterAsync([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var authResult = await _authService.RegisterAsync(request.Email, request.Password, cancellationToken);
        return authResult is null ? BadRequest("Email already exists") : Ok(authResult);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshTokenAsync([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        var authResult = await _authService.GetRefreshTokenAsync(request.Token, request.RefreshToken, cancellationToken);
        return authResult is null ? BadRequest("Invalid token/refresh token") : Ok(authResult);
    }

    [HttpPut("revoke-refresh-token")]
    public async Task<IActionResult> RevokeRefreshTokenAsync([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        var result = await _authService.RevokeRefreshTokenAsync(request.Token, request.RefreshToken, cancellationToken);
        return result ? Ok() : BadRequest("Invalid token/refresh token");
    }

}