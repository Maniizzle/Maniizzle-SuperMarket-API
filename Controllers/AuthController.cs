using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;

//using DatingAppAPI.Services;
//using DatingAppAPI.Dtos;
using Supermarket.API.Domain.Model;
using Supermarket.API.Domain.Repositories;
using Supermarket.API.Dtos;
using Microsoft.AspNetCore.Authorization;

namespace Supermarket.API.Controllers
{
    [Route("api/auth/")]
    [ApiController]
    [Authorize]
    public class AuthController : Controller
    {
        private readonly IAuthRepository _repository;

        private readonly IConfiguration _configuration;

        public AuthController(IAuthRepository repository, IConfiguration configuration)
        {
            _repository = repository;
            _configuration = configuration;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register(UserForRegisterDto userforRegisterDto)
        {
            if (string.IsNullOrEmpty(userforRegisterDto.Username))
                userforRegisterDto.Username = userforRegisterDto.Username.ToLower();

            if (await _repository.UserExists(userforRegisterDto.Username))
                ModelState.AddModelError("Username", "Username already exist");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            userforRegisterDto.Username = userforRegisterDto.Username.ToLower();
            if (await _repository.UserExists(userforRegisterDto.Username))
                return BadRequest("Username is already token ");

            var userToCreate = new User
            {
                Username = userforRegisterDto.Username
            };
            var createUser = await _repository.Register(userToCreate, userforRegisterDto.Password);
            return StatusCode(201);
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(UserForLoginDto userForLoginDto)
        {
            //throw new Exception("Computer says NO!");
            var userFromRepo = await _repository.Login(userForLoginDto.Username.ToLowerInvariant(), userForLoginDto.Password);
            if (userFromRepo == null)
                return Unauthorized();

            //generate token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes((_configuration.GetSection("AppSettings:token").Value));
            // var checkkey = Convert.FromBase64String("super secret key");
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new System.Security.Claims.ClaimsIdentity(new Claim[]
                 {
                     new Claim(ClaimTypes.NameIdentifier, userFromRepo.Id.ToString()),
                     new Claim(ClaimTypes.Name, userFromRepo.Username)
                 }),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);
            return Ok(new { tokenString });
        }
    }
}