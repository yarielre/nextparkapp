using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Inside.Domain.Models;
using Inside.Web.Infrastructure;
using Inside.Web.Models;
using Inside.Web.Repositories;
using Inside.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Inside.Web.Controllers.Api
{
    [Route("api/[controller]/[action]")]
    public class AuthController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly EmailSender _emailSender;
        private readonly IMapper _mapper;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<ApplicationUser> _useRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly PushNotificationService _pushNotificationService;

        public AuthController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager, IConfiguration configuration,
            IMapper mapper, IEmailSender emailSender, IRepository<ApplicationUser> useRepository,
            IUnitOfWork unitOfWork, PushNotificationService pushNotificationService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _mapper = mapper;
            _emailSender = emailSender as EmailSender;
            _useRepository = useRepository;
            _unitOfWork = unitOfWork;
            _pushNotificationService = pushNotificationService;
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, false, false);

            if (result.Succeeded)
            {
                var appUser = _userManager.Users.SingleOrDefault(r => r.UserName == model.UserName);
                return Ok(GenerateJwtTokenAsync(model.UserName, appUser));
            }
            //TODO: Remove
            _emailSender.SendDebugMessage("Auth", "Login", "INVALID_LOGIN_ATTEMPT user " + model.UserName);

            return BadRequest("INVALID_LOGIN_ATTEMPT");
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            var user = new ApplicationUser
            {
                UserName = model.Username,
                Name = model.Name,
                Lastname = model.Lastname,
                Email = model.Email,
                Address = model.Address,
                CarPlate = model.CarPlate,               
                State = model.State
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                //TODO: Remove
                await _signInManager.SignInAsync(user, false);
                _emailSender.SendDebugMessage("Auth", "Register", "Registration ok " + model.Username);
                return Ok(GenerateJwtTokenAsync(model.Email, user));
            }
            //TODO: Remove
            _emailSender.SendDebugMessage("Auth", "Register",
                $"Server Error: {result.Errors.FirstOrDefault()?.Code}\n{result.Errors.FirstOrDefault()?.Description}");
            return BadRequest(
                $"Server Error: {result.Errors.FirstOrDefault()?.Code}\n{result.Errors.FirstOrDefault()?.Description}");
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return Ok();
        }

        private TokenResponse GenerateJwtTokenAsync(string email, ApplicationUser user)
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
            var tokenResponse = new TokenResponse
            {
                UserId = user.Id,
                AuthToken = new JwtSecurityTokenHandler().WriteToken(token),
                IsSuccess = true
            };

            return tokenResponse;
        }

        [HttpPost]
        //[Authorize]
        public IActionResult GetUserByUserName([FromBody] string username)
        {
            if (ModelState.IsValid)
            {
                var appUser = _userManager.Users.SingleOrDefault(r => r.UserName == username);
                var userVm = _mapper.Map<ApplicationUser, UserModel>(appUser);
                return Ok(userVm);
            }
            return BadRequest(ModelState);
        }
        //[Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordModel model)
        {
            var user = _userManager.Users.FirstOrDefault(u => u.Id == model.Id);
            if (user == null)
                return BadRequest("User not found.");
            var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            if (result.Succeeded)
                return Ok();
            return BadRequest(result.Errors.FirstOrDefault()?.Description);
        }

        [HttpPost]
        //[Authorize]
        public async Task<IActionResult> EditProfile([FromBody] EditProfileModel model)
        {
            var user = _userManager.Users.SingleOrDefault(r => r.Id == model.Id);

            user.UserName = model.Username;
            user.Name = model.Name;
            user.Lastname = model.Lastname;
            user.Email = model.Email;
            user.Address = model.Address;
            user.CarPlate = model.CarPlate;          
            user.State = model.State;

            var passwordCheckResult = await _userManager.CheckPasswordAsync(user, model.OldPassword);
            if (!passwordCheckResult)
                return BadRequest();
            var passwordChnageResult =
                await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
                return Ok();
            return BadRequest(
                $"Server Error: {result.Errors.FirstOrDefault()?.Code}\n{result.Errors.FirstOrDefault()?.Description}");
        }

        [HttpPost]
        //[Authorize]
        public async Task<ActionResult> UpdateCoin([FromBody] UpdateUserCoinModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var user = _useRepository.Find(model.UserId);
            user.Coins = model.Coins;
            _useRepository.Update(user);
            await _unitOfWork.CommitAsync();
            var vm = _mapper.Map<ApplicationUser, UserModel>(user);
            return Ok(vm);
        }
    }
}