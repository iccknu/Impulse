using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

using ImpulseAPI.Models;

namespace ImpulseAPI.Controllers
{
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        [AllowAnonymous]
        [HttpPost]
        public IActionResult RequestToken([FromBody] TokenRequestModel request)
        {
            if (request.Username == "login" && request.Password == "password")
            {
                var claims = new[]
                {
            new Claim(ClaimTypes.Name, request.Username)
        };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("SuperKeyftyfthfhfghffhgfhgfhgfhgfghf"));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: "localhost",
                    audience: "localhost",
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(10),
                    signingCredentials: creds);

                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token)
                });
            }

            return BadRequest("Could not verify username and password");
        }
    }
}