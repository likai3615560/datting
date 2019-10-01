using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DatingApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repo;
        private readonly IConfiguration _config;

        public AuthController(IAuthRepository repo, IConfiguration config)
        {
            _config = config;
            _repo = repo;

        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserForRegisterDto userforregisterdto)
        {

            userforregisterdto.username = userforregisterdto.username.ToLower();
            if (await _repo.UserExits(userforregisterdto.username))
            {
                return BadRequest("username already exists");
            }
            var usertocreate = new User()
            {
                Username = userforregisterdto.username
            };

            var createuser = await _repo.Register(usertocreate, userforregisterdto.password);
            return StatusCode(201);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserForLoginDtos userforlogindtos)
        {
            var userformrep = await _repo.Login(userforlogindtos.Username, userforlogindtos.Password);
            if (userformrep == null)
                return Unauthorized();

            var clams = new[]{
                new Claim(ClaimTypes.NameIdentifier,userformrep.Id.ToString()),
                new Claim(ClaimTypes.Name,userformrep.Username)

            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokendescriptor = new SecurityTokenDescriptor
            {

                Subject = new ClaimsIdentity(clams),
                Expires = DateTime.Now.Date.AddDays(1),
                SigningCredentials = creds
            };

            var tokenhander = new JwtSecurityTokenHandler();
            var token = tokenhander.CreateToken(tokendescriptor);

            return Ok(new
            {

                token = tokenhander.WriteToken(token)
            });
        }
    }
}