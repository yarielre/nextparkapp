using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NextPark.Data.Infrastructure;
using NextPark.Data.Repositories;
using NextPark.Domain.Entities;
using NextPark.Enums;
using NextPark.Enums.Enums;
using NextPark.Models;
using NextPark.Services;

namespace NextPark.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PurchaseController : ControllerBase
    {
        private readonly IHostingEnvironment _appEnvironment;
        private readonly IEmailSender _emailSender;
        private readonly IMapper _mapper;

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;

        public PurchaseController(IUnitOfWork unitOfWork, IMapper mapper,
            IHostingEnvironment appEnvironment,
            IEmailSender emailSender,
            UserManager<ApplicationUser> userManager)
        {
            _appEnvironment = appEnvironment;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _emailSender = emailSender;
            _userManager = userManager;
        }

        // POST api/controller
        [HttpPost("amount")]
        public async Task<IActionResult> BuyAmount([FromBody] PurchaseModel model)
        {

            if (model == null) return BadRequest("Model not valid");

            var user = _userManager.Users.FirstOrDefault(r => r.Id == model.UserId);

            if (user == null) return BadRequest("User not found");

            user.Balance =+ model.CashToAdd;

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded) {

                model.NewUserBalance = user.Balance;

                return Ok(model);
            }

            return BadRequest(result.Errors);
          
        }

        // POST api/controller
        [HttpPost("drawal")]
        public IActionResult DrawalCash([FromBody] PurchaseModel model)
        {
            return Ok();
        }
    }
}