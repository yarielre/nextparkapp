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
    public class ProfileController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly EmailSender _emailSender;
        private readonly IMapper _mapper;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<ApplicationUser> _useRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProfileController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager, IConfiguration configuration,
            IMapper mapper, IEmailSender emailSender, IRepository<ApplicationUser> useRepository,
            IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _mapper = mapper;
            _emailSender = emailSender as EmailSender;
            _useRepository = useRepository;
            _unitOfWork = unitOfWork;
        }

        [HttpPost("editpass")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordModel model)
        {

            if (model == null)
            {
                return BadRequest("Invalid ChangePasswordModel parameter");
            }

            var user = _userManager.Users.FirstOrDefault(u => u.Id == model.Id);

            if (user == null)
            {
                return BadRequest("User not found.");
            }

            try
            {

                var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);

                if (result.Succeeded)
                {
                    return Ok();
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

        [HttpPost("edit")]
        public async Task<IActionResult> EditProfile([FromBody] EditProfileModel model)
        {
            if (model == null)
            {
                return BadRequest("Invalid EditProfileModel parameter");
            }

            var user = _userManager.Users.SingleOrDefault(r => r.Id == model.Id);

            if (user == null)
            {
                return BadRequest("User not found.");
            }

            try
            {
                user.UserName = model.Username;
                user.Name = model.Name;
                user.Lastname = model.Lastname;
                user.Email = model.Email;
                user.Address = model.Address;
                user.CarPlate = model.CarPlate;
                user.State = model.State;

                var passwordCheckResult = await _userManager.CheckPasswordAsync(user, model.OldPassword);

                if (!passwordCheckResult)
                {
                    return BadRequest("Invalid previous password");
                }

                var passwordChangeResult = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);

                if (!passwordChangeResult.Succeeded)
                {

                    return BadRequest("Impossible to change the password!");
                }

                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    return Ok();
                }
                else
                {

                    return BadRequest("Impossible to update the user!");
                }
            }
            catch (Exception e)
            {
                return BadRequest(string.Format("Server error: {0}", e));
            }
        }

        [HttpPost("editcoins")]
        public async Task<ActionResult> UpdateCoin([FromBody] UpdateUserCoinModel model)
        {
            if (model == null)
            {
                return BadRequest("Invalid UpdateUserCoinModel parameter");
            }

            var user = _useRepository.Find(model.UserId);

            if (user == null)
            {
                return BadRequest("User not found.");
            }
            try
            {
                user.Balance = model.Coins;
                _useRepository.Update(user);
                await _unitOfWork.CommitAsync();
                var vm = _mapper.Map<ApplicationUser, UserModel>(user);
                return Ok(vm);

            }
            catch (Exception e)
            {
                return BadRequest(string.Format("Server error: {0}", e));
            }

        }
    }
}