using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NextPark.Data.Infrastructure;
using NextPark.Data.Repositories;
using NextPark.Domain.Entities;
using NextPark.Enums.Enums;
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
    public class PurchaseController : ControllerBase
    {
        private readonly IHostingEnvironment _appEnvironment;
        private readonly IEmailSender _emailSender;
        private readonly IMapper _mapper;
        private readonly IRepository<Transaction> _transactionRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;

        public PurchaseController(IUnitOfWork unitOfWork, IMapper mapper,
            IHostingEnvironment appEnvironment,
            IEmailSender emailSender,
            UserManager<ApplicationUser> userManager,
            IRepository<Transaction> transactionRepository)
        {
            _appEnvironment = appEnvironment;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _emailSender = emailSender;
            _userManager = userManager;
            _transactionRepository = transactionRepository;
        }


        // POST api/controller
        [HttpPost("amount")]
        public async Task<IActionResult> BuyAmount([FromBody] PurchaseModel model)
        {

            if (model == null) return BadRequest("Model not valid");

            var user = _userManager.Users.FirstOrDefault(r => r.Id == model.UserId);

            if (user == null) return BadRequest("User not found");

            user.Balance = user.Balance + model.Cash;

            try
            {
                var increaseBalanceTransaction = new Transaction
                {
                    CashMoved = model.Cash,
                    UserId = user.Id,
                    CreationDate = DateTime.Now,
                    CompletationDate = DateTime.Now,
                    Status = TransactionStatus.Completed,
                    TransactionId = new Guid(),
                    Type = TransactionType.IncreaseBalanceTransaction
                };

                _transactionRepository.Add(increaseBalanceTransaction);

                await _userManager.UpdateAsync(user).ConfigureAwait(false);
                await _unitOfWork.CommitAsync().ConfigureAwait(false);
                
                model.NewUserBalance = user.Balance;

                return Ok(model);
            }
            catch (Exception e)
            {
                return BadRequest($"Server error increasing user balance: {e.Message}");
            }

        }

        // POST api/controller
        [HttpPost("drawal")]
        public async Task<IActionResult> DrawalCash([FromBody] PurchaseModel model)
        {
            if (model == null) return BadRequest("Model not valid");
            var user = _userManager.Users.FirstOrDefault(u => u.Id == model.UserId);
            if (user == null)
                return BadRequest("User not found");
            if (user.Profit < model.Cash)
            {
                return BadRequest("Your balance is smaller than the cash you want to move.");
            }
            user.Profit = user.Profit - model.Cash;

            try
            {
                var drawalTransaction = new Transaction
                {
                    CashMoved = model.Cash,
                    UserId = user.Id,
                    CreationDate = DateTime.Now,
                    CompletationDate = DateTime.Now,
                    Status = TransactionStatus.Pending,
                    TransactionId = new Guid(),
                    Type = TransactionType.WithdrawalTransaction
                };
                _transactionRepository.Add(drawalTransaction);
                await _userManager.UpdateAsync(user).ConfigureAwait(false);
                await _unitOfWork.CommitAsync().ConfigureAwait(false);
                model.NewUserProfit = user.Profit;
                return Ok(model);
            }
            catch (Exception e)
            {
                return BadRequest($"Server error increasing user balance: {e.Message}");
            }
        }
    }
}