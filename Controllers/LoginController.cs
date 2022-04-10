using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using netCorePlayground.DTOs;

namespace netCorePlayground.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly IConfiguration _config;

        private IList<Models.User> appUsers = new List<Models.User>
        {
            new Models.User
            {
                FullName = "Admin", UserName = "admin", Password = "1234",
                UserRoles = new List<string>
                    {Infra.Policies.UserPolicy, Infra.Policies.TesterUserRole, Infra.Policies.AdminRole}
            },
            new Models.User
            {
                FullName = "John Doe", UserName = "user", Password = "1234",
                UserRoles = new List<string> {Infra.Policies.UserPolicy}
            },
            new Models.User
            {
                FullName = "Tester", UserName = "tester", Password = "1234",
                UserRoles = new List<string>
                    {Infra.Policies.UserPolicy, Infra.Policies.TesterUserRole}
            },
        };

        public LoginController(IConfiguration config)
        {
            _config = config;
        }

        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType((int) HttpStatusCode.OK, Type = typeof(LoginOutputDTO))]
        public IActionResult Login([FromBody] LoginInputDTO login)
        {
            Models.User user = AuthenticateUser(login);
            if (user == null)
            {
                return Unauthorized();
            }

            return Ok(new LoginOutputDTO
            {
                AccessToken = "Bearer " + GenerateJwtToken(user)
            });
        }

        private Models.User AuthenticateUser(LoginInputDTO loginCredentials)
        {
            Models.User user = appUsers.SingleOrDefault(x =>
                x.UserName == loginCredentials.Username && x.Password == loginCredentials.Password);

            return user;
        }

        private string GenerateJwtToken(Models.User userInfo)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:SecretKey"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, userInfo.UserName),
                new Claim(ClaimTypes.Name, userInfo.FullName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            foreach (var userRole in userInfo.UserRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, userRole));
            }

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(4),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}