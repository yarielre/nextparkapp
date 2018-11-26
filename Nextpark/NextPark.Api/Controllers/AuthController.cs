using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NextPark.Data.Infrastructure;
using NextPark.Data.Repositories;
using NextPark.Domain.Entities;
using NextPark.Models;
using NextPark.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace NextPark.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly EmailSender _emailSender;
        private readonly IMapper _mapper;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<ApplicationUser> _useRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly PushNotificationService _pushNotificationService;
        private readonly IAuthService _authService;

        public AuthController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager, IConfiguration configuration,
            IMapper mapper, IEmailSender emailSender, IRepository<ApplicationUser> useRepository,
            IUnitOfWork unitOfWork, PushNotificationService pushNotificationService,
            IAuthService authService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _mapper = mapper;
            _emailSender = emailSender as EmailSender;
            _useRepository = useRepository;
            _unitOfWork = unitOfWork;
            _pushNotificationService = pushNotificationService;
            _authService = authService;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            if (model == null || 
                string.IsNullOrEmpty(model.UserName) ||
                string.IsNullOrEmpty(model.Password))
            {
                return BadRequest("Invalid LoginModel parameter");
            }

            try
            {
                var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, false, false);

                if (result.Succeeded)
                {
                    var appUser = _userManager.Users.SingleOrDefault(r => r.UserName == model.UserName);
                    return Ok(_authService.GenerateJwtTokenAsync(model.UserName, appUser));
                }
                else
                {
                    return BadRequest(result);
                }
            }
            catch (Exception e)
            {
                return BadRequest(string.Format("Server error: {0}", e));
            }
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            if (model == null)
            {
                return BadRequest("Invalid RegisterModel parameter");
            }

            try
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
                    return Ok(_authService.GenerateJwtTokenAsync(model.Email, user));
                }
                else
                {
                    return BadRequest(result);
                }
            }
            catch (Exception e)
            {
                return BadRequest(string.Format("Server error: {0}", e));
            }

        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok();
        }

        [HttpPost]
        public IActionResult GetUserByUserName([FromBody] string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return BadRequest("Invalid username parameter");
            }
            try
            {
                var appUser = _userManager.Users.SingleOrDefault(r => r.UserName == username);

                var userVm = _mapper.Map<ApplicationUser, UserModel>(appUser);

                return Ok(userVm);
            }
            catch (Exception e)
            {
                return BadRequest(string.Format("Server error: {0}", e));
            }
        }




    }
}