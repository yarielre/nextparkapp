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
using NextPark.Enums.Enums;

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
        private readonly IMediaService _mediaService;

        public ProfileController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager, IConfiguration configuration,
            IMapper mapper, IEmailSender emailSender, IRepository<ApplicationUser> useRepository,
            IUnitOfWork unitOfWork, IMediaService mediaService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _mapper = mapper;
            _emailSender = emailSender as EmailSender;
            _useRepository = useRepository;
            _unitOfWork = unitOfWork;
            _mediaService = mediaService;
        }

        [HttpPost("editpass")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordModel model)
        {

            if (model == null)
            {
                return BadRequest(ApiResponse.GetErrorResponse("Invalid ChangePasswordModel parameter",ErrorType.EntityNull));
            }
            var user = _userManager.Users.FirstOrDefault(u => u.Id == model.Id);

            if (user == null)
            {
                return BadRequest(ApiResponse.GetErrorResponse("User not found.",ErrorType.EntityNotFound));
            }

            try
            {
                var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword).ConfigureAwait(false);

                if (result.Succeeded)
                {
                    return Ok(ApiResponse.GetSuccessResponse(true, "Ok"));
                }
                return BadRequest(ApiResponse.GetErrorResponse(result.Errors.FirstOrDefault()?.Description,ErrorType.ChangePasswordError));
            }
            catch (Exception e)
            {
                return BadRequest(ApiResponse.GetErrorResponse(e.Message,ErrorType.Exception));
            }
        }

        [HttpPost("edit")]
        public async Task<IActionResult> EditProfile([FromBody] EditProfileModel model)
        {
            if (model == null)
            {
                return BadRequest(ApiResponse.GetErrorResponse("Invalid EditProfileModel parameter",ErrorType.EntityNull));
            }

            var user = _userManager.Users.SingleOrDefault(r => r.Id == model.Id);
            if (user == null)
            {
                return BadRequest(ApiResponse.GetErrorResponse("User not found.",ErrorType.EntityNotFound));
            }

            try
            {
                var imageUrl = _mediaService.SaveImage(model.ImageBinary);
                if (!string.IsNullOrEmpty(imageUrl)) model.ImageUrl = imageUrl;
            }
            catch (Exception e)
            {
                return BadRequest(ApiResponse.GetErrorResponse(e.Message, ErrorType.Exception));
                //Log: return BadRequest(string.Format("{0} Exception: {1}", "Error processing Image!", e.Message));
            }

            try
            {
                user.Name = model.Name;
                user.Lastname = model.Lastname;
                user.Email = model.Email;
                user.Address = model.Address;
                user.CarPlate = model.CarPlate;
                user.State = model.State;
                user.Phone = model.Phone;
                user.Cap = model.Cap;
                user.City = model.City;
                user.ImageUrl = model.ImageUrl;

                var result = await _userManager.UpdateAsync(user).ConfigureAwait(false);

                if (result.Succeeded)
                {
                    var userVm = _mapper.Map<ApplicationUser, UserModel>(user);
                    return Ok(ApiResponse.GetSuccessResponse(userVm));
                }
                return BadRequest(ApiResponse.GetErrorResponse("Impossible to update the user!",ErrorType.None));
            }
            catch (Exception e)
            {
                return BadRequest(ApiResponse.GetErrorResponse(e.Message,ErrorType.Exception));
            }
        }
    }
}