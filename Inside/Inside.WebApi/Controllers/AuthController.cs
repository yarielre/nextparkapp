using AutoMapper;
using Inside.Domain.Entities;
using Inside.Domain.Models;
using Inside.WebApi.Helpers;
using Inside.WebApi.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Inside.WebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    public class AuthController : Controller
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public AuthController(UserManager<User> userManager, SignInManager<User> signInManager, IConfiguration configuration, IMapper mapper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _mapper = mapper;

        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody]LoginModel model)
        {
            var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, false, false);

            if (result.Succeeded)
            {
                var appUser = _userManager.Users.SingleOrDefault(r => r.UserName == model.UserName);
                return Ok(GenerateJwtTokenAsync(model.UserName, appUser));
            }

            return BadRequest("INVALID_LOGIN_ATTEMPT");
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            var user = new User
            {
                UserName = model.Username,
                Email = model.Email,
                Address = model.Address,
                CarPlate = model.CarPlate,
                Lastname = model.Lastname,
                Name = model.Name,
                State = model.State
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, false);
                return Ok(GenerateJwtTokenAsync(model.Email, user));

            }
            return BadRequest($"Server Error: {result.Errors.FirstOrDefault()?.Code}\n{result.Errors.FirstOrDefault()?.Description}");
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return Ok();
        }

        private TokenResult GenerateJwtTokenAsync(string email, User user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                //  new Claim(JwtRegisteredClaimNames.Email, email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddDays(Convert.ToDouble(_configuration["JwtExpireDays"]));

            var token = new JwtSecurityToken(
                _configuration["JwtIssuer"],
                _configuration["JwtIssuer"],
                claims,
                expires: expires,
                signingCredentials: creds
            );
            return new TokenResult(new JwtSecurityTokenHandler().WriteToken(token), user.Id);
        }

        [HttpPost]
        public IActionResult GetUserByUserName([FromBody]string username)
        {
            if (ModelState.IsValid)
            {
                var appUser = _userManager.Users.SingleOrDefault(r => r.UserName == username);
                var userVm = _mapper.Map<User, UserModel>(appUser);
                return Ok(userVm);
            }
            return BadRequest(ModelState);
        }
        [HttpPost]
        public async Task<IActionResult> ChangePassword([FromBody]ChangePasswordViewModel model)
        {
            var user = _userManager.Users.FirstOrDefault(u => u.Id == model.Id);
            if (user == null)
            {
                return BadRequest("User not found.");
            }
            var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                return Ok();
            }
            return BadRequest(result.Errors.FirstOrDefault()?.Description);
        }

        [HttpPost]
        public async Task<IActionResult> EditProfile([FromBody] EditProfileModel model)
        {
            var user = _userManager.Users.SingleOrDefault(r => r.Id == model.Id);

            user.UserName = model.Username;
            user.Email = model.Email;
            user.Address = model.Address;
            user.CarPlate = model.CarPlate;
            user.Lastname = model.Lastname;
            user.Name = model.Name;
            user.State = model.State;

            var passwordCheckResult = await _userManager.CheckPasswordAsync(user, model.OldPassword);
            if (!passwordCheckResult)
            {
                //Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary a = new Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary();
                //a.AddModelError("PasswordError","OLD_PASSWORD_INCORRECT");
                return BadRequest();
                // No se como enviar un mensaje en especifico
            }
            var passwordChnageResult = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return Ok();
            }
            return BadRequest($"Server Error: {result.Errors.FirstOrDefault()?.Code}\n{result.Errors.FirstOrDefault()?.Description}");
        }
    }
}

