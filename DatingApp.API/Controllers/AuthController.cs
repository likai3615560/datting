using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace DatingApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repo;

        public AuthController(IAuthRepository repo)
        {
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
    }
}