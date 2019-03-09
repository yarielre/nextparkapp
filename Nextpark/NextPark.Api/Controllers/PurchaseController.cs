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

        [HttpPut("{id}/addbalance")]
        public async Task<IActionResult> AddBalance(int id, [FromBody] double balance)
        {
            var user = _userManager.Users.FirstOrDefault(u => u.Id == id);
            if (user == null)
                return BadRequest("User not found");
            user.Balance = user.Balance + balance;
            try
            {
                var increaseBalanceTransaction = new Transaction
                {
                    CashMoved = balance,
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
                var userVm = _mapper.Map<ApplicationUser, UserModel>(user);
                return Ok(userVm);
            }
            catch (Exception e)
            {
                return BadRequest($"Server error increasing user balance: {e.Message}");
            }
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

            if (result.Succeeded)
            {

                model.NewUserBalance = user.Balance;

                return Ok(model);
            }

            return BadRequest(result.Errors);

        }

        // POST api/controller
        [HttpPost("{id}/drawal")]
        public async Task<IActionResult> DrawalCash(int id, [FromBody] double amount)
        {
            var user = _userManager.Users.FirstOrDefault(u => u.Id == id);
            if (user == null)
                return BadRequest("User not found");
            try
            {
                var drawalTransaction = new Transaction
                {
                    CashMoved = amount,
                    UserId = user.Id,
                    CreationDate = DateTime.Now,
                    CompletationDate = DateTime.Now,
                    Status = TransactionStatus.Pending,
                    TransactionId = new Guid(),
                    Type = TransactionType.WithdrawalTransaction
                };
                _transactionRepository.Add(drawalTransaction);
                await _unitOfWork.CommitAsync().ConfigureAwait(false);
                return Ok(amount);
            }
            catch (Exception e)
            {
                return BadRequest($"Server error increasing user balance: {e.Message}");
            }
        }
    }
}