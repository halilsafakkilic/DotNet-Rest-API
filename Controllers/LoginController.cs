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
using netCorePlayground.Models;

namespace JWTAuthenticationExample.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly IConfiguration _config;

        private IList<User> appUsers = new List<User>
        {
            new User { FullName = "Admin", UserName = "admin", Password = "1234", UserRoles = new List<string> { netCorePlayground.Infra.Policies.ViewRole, netCorePlayground.Infra.Policies.AdminPolicy } },
            new User { FullName = "Guest", UserName = "guest", Password = "1234", UserRoles = new List<string> { netCorePlayground.Infra.Policies.UserPolicy } },
            new User { FullName = "Client", UserName = "user", Password = "1234", UserRoles = new List<string> { netCorePlayground.Infra.Policies.ViewRole, netCorePlayground.Infra.Policies.UserPolicy } }
        };

        public LoginController(IConfiguration config)
        {
            _config = config;
        }

        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(LoginOutputDTO))]
        public IActionResult Login([FromBody] LoginInputDTO login)
        {
            User user = AuthenticateUser(login);
            if (user == null)
            {
                return Unauthorized();
            }

            return Ok(new LoginOutputDTO
            {
                Token = GenerateJWTToken(user)
            });
        }

        User AuthenticateUser(LoginInputDTO loginCredentials)
        {
            User user = appUsers.SingleOrDefault(x => x.UserName == loginCredentials.UserName && x.Password == loginCredentials.Password);

            return user;
        }

        string GenerateJWTToken(User userInfo)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:SecretKey"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, userInfo.UserName),
                new Claim(ClaimTypes.Name, userInfo.FullName.ToString()),
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
                expires: DateTime.Now.AddHours(1),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}