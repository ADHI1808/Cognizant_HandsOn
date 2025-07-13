using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MyFirstAPI.filters
{
    public class CustomAuthFilter : ActionFilterAttribute
    {
        private readonly ILogger<CustomAuthFilter> _logger;
        private readonly IConfiguration _configuration;

        public CustomAuthFilter(ILogger<CustomAuthFilter> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.HttpContext.Request.Headers.TryGetValue("Authorization", out var authorizationHeader))
            {
                _logger.LogWarning("Authorization header is missing.");
                context.Result = new BadRequestObjectResult("Invalid request - No Auth token");
                return;
            }

            string authorizationHeaderValue = authorizationHeader.ToString();

            if (!authorizationHeaderValue.StartsWith("Bearer "))
            {
                _logger.LogWarning($"Authorization header value does not start with 'Bearer '. Value: {authorizationHeaderValue}");
                context.Result = new BadRequestObjectResult("Invalid request - Token present but Bearer unavailable");
                return;
            }

            string token = authorizationHeaderValue.Substring("Bearer ".Length);

            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = GetTokenValidationParameters();

            try
            {
                SecurityToken validatedToken;
                var principal = tokenHandler.ValidateToken(token, validationParameters, out validatedToken);

                var roles = principal.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToList();
                bool hasAdminRole = roles.Contains("Admin");
                bool hasPocRole = roles.Contains("POC");

                var isEmployeeGetRequest = context.ActionDescriptor.RouteValues["controller"] == "Employee" &&
                                           context.ActionDescriptor.RouteValues["action"] == "Get";

                if (isEmployeeGetRequest)
                {
                    if (!hasPocRole)
                    {
                        _logger.LogWarning("Access denied for Employee.Get(): 'POC' role is required but not present in the token.");
                        context.Result = new UnauthorizedObjectResult("Access denied: 'POC' role is required.");
                        return;
                    }

                    if (hasPocRole && hasAdminRole)
                    {
                        _logger.LogInformation("Access granted for Employee.Get(): Required roles 'Admin' and 'POC' are present.");
                    }
                    else if (hasPocRole && !hasAdminRole)
                    {
                        _logger.LogWarning("Access denied for Employee.Get(): 'Admin' role is required along with 'POC' but not present in the token.");
                        context.Result = new UnauthorizedObjectResult("Access denied: 'Admin' role is required.");
                        return;
                    }
                }
            }
            catch (SecurityTokenValidationException ex)
            {
                _logger.LogError($"Token validation failed: {ex.Message}");
                context.Result = new UnauthorizedObjectResult("Invalid token.");
                return;
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred during token processing: {ex.Message}");
                context.Result = new StatusCodeResult(StatusCodes.Status500InternalServerError);
                return;
            }

            base.OnActionExecuting(context);
        }

        private TokenValidationParameters GetTokenValidationParameters()
        {
            var securityKeyBytes = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);
            var issuer = _configuration["Jwt:Issuer"];
            var audience = _configuration["Jwt:Audience"];

            return new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(securityKeyBytes),
                ValidateIssuer = true,
                ValidIssuer = issuer,
                ValidateAudience = true,
                ValidAudience = audience,
                ClockSkew = TimeSpan.FromMinutes(5)
            };
        }
    }
}