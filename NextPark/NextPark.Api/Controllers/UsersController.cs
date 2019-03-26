using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NextPark.Data.Infrastructure;
using NextPark.Data.Repositories;
using NextPark.Domain.Entities;
using NextPark.Models;
using NextPark.Models.Models.HelpersModel;
using NextPark.Services;

namespace NextPark.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IEmailSender _emailSender;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfwork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRepository<ApplicationUser> _userRepository;

        public UsersController(UserManager<ApplicationUser> userManager, IMapper mapper, IRepository<ApplicationUser> userRepository,
            IUnitOfWork unitOfWork, IEmailSender emailSender)
        {
            _userManager = userManager;
            _mapper = mapper;
            _userRepository = userRepository;
            _emailSender = emailSender;
            _unitOfwork = unitOfWork;
        }

        // GET: api/User
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var appUserList = _userManager.Users.ToList();
                var appUserMapped = appUserList.Select(x => _mapper.Map<ApplicationUser, UserModel>(x));
                return Ok(appUserMapped);
            }
            catch (Exception e)
            {
                return BadRequest(string.Format("Server error: {0}", e));
            }
        }

        // GET: api/User/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/User
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        [HttpPost("sendtoall")]
        public async Task<IActionResult> SendEmailToAllClients([FromBody] EmailSubjectMessage model)
        {
            if (!ModelState.IsValid || string.IsNullOrEmpty(model.Message))
                return BadRequest();
            var users = _userManager.Users.ToList();
            foreach (var user in users)
                try
                {
                    await _emailSender.SendEmailAsync(user.Email, model.Subject, model.Message).ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    return BadRequest("Error sending email to all clients");
                }
            return Ok();
        }

        [HttpPost("sendtoone")]
        public async Task<IActionResult> SendEmailToOneClient([FromBody] EmailSubjectMessage model)
        {
            if (!ModelState.IsValid || string.IsNullOrEmpty(model.Message) || model.Id <= 0)
                return BadRequest();
            var user = _userManager.Users.FirstOrDefault(u => u.Id == model.Id);
            if (user == null)
                return BadRequest("User not found");

            try
            {
                await _emailSender.SendEmailAsync(user.Email, model.Subject, model.Message).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                return BadRequest($"Error sending email to client {user.Email}");
            }

            return Ok();
        }

        // PUT: api/User/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] UserModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var user = _userManager.Users.FirstOrDefault(u => u.Id == id);
            if (user == null)
                return BadRequest("User not found");

            user.Name = model.Name;
            user.Lastname = model.Lastname;
            user.UserName = model.UserName;
            user.Phone = model.Phone;
            user.City = model.City;
            _userRepository.Update(user);
            await _unitOfwork.CommitAsync().ConfigureAwait(false);
            var userModel = _mapper.Map<ApplicationUser, UserModel>(user);
            return Ok(userModel);
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == id).ConfigureAwait(false);
            var userAngularModel = _mapper.Map<ApplicationUser, UserModel>(user);
            if (user == null)
                return BadRequest("User not found");
            _userRepository.Delete(user);
            await _unitOfwork.CommitAsync().ConfigureAwait(false);
            return Ok(userAngularModel);
        }
    }
}