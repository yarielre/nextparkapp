using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NextPark.Data.Infrastructure;
using NextPark.Data.Repositories;
using NextPark.Domain.Entities;
using NextPark.Enums.Enums;
using NextPark.Models;
using NextPark.Models.Models;

namespace NextPark.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        private readonly IRepository<Transaction> _transactionRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public TransactionsController(IRepository<Transaction> transactionRepository, IMapper mapper, IUnitOfWork unitOfWork)
        {
            _transactionRepository = transactionRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        // GET: api/Transactions
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var transactions = await _transactionRepository.FindAllAsync(new CancellationToken(),t=>t.User).ConfigureAwait(false);
            var vm = _mapper.Map<List<Transaction>, List<TransactionModel>>(transactions);
            return Ok(ApiResponse<List<TransactionModel>>.GetSuccessResponse(vm));
        }

        // GET: api/Transactions/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var entity = _transactionRepository.Find(id);
            if (entity == null)
                return BadRequest(ApiResponse<TransactionModel>.GetErrorResponse("Entity not found", ErrorType.EntityNotFound));
            var vm = _mapper.Map<Transaction, TransactionModel>(entity);
            return Ok(ApiResponse<TransactionModel>.GetSuccessResponse(vm));
        }

        // POST: api/Transactions
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] TransactionModel model)
        {
            if (model == null)
            {
                return BadRequest(
                    ApiResponse<List<TransactionModel>>.GetErrorResponse("Model is null", ErrorType.EntityNull));
            }
            try
            {
                var transaction = _mapper.Map<TransactionModel, Transaction>(model);
                _transactionRepository.Add(transaction);
                await _unitOfWork.CommitAsync().ConfigureAwait(false);
                var vm = _mapper.Map<Transaction, TransactionModel>(transaction);
                return Ok(ApiResponse<TransactionModel>.GetSuccessResponse(vm));
            }
            catch (Exception e)
            {
                return BadRequest(
                    ApiResponse<List<TransactionModel>>.GetErrorResponse(e.Message, ErrorType.Exception));
            }
        }

        // PUT: api/Transactions/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] TransactionModel model)
        {
            if (model == null)
            {
                return BadRequest(
                    ApiResponse<List<TransactionModel>>.GetErrorResponse("Model is null", ErrorType.EntityNull));
            }
            try
            {
                var transaction = _mapper.Map<TransactionModel, Transaction>(model);
                _transactionRepository.Update(transaction);
                await _unitOfWork.CommitAsync().ConfigureAwait(false);
                var vm = _mapper.Map<Transaction, TransactionModel>(transaction);
                return Ok(ApiResponse<TransactionModel>.GetSuccessResponse(vm));
            }
            catch (Exception e)
            {
                return BadRequest(
                    ApiResponse<List<TransactionModel>>.GetErrorResponse(e.Message, ErrorType.Exception));
            }
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = _transactionRepository.Find(id);
            if (entity == null)
            {
                return BadRequest(ApiResponse<TransactionModel>.GetErrorResponse("Can't deleted, entity not found.", ErrorType.EntityNotFound));

            }
            var vm = _mapper.Map<Transaction, TransactionModel>(entity);
            _transactionRepository.Delete(entity);

            await _unitOfWork.CommitAsync().ConfigureAwait(false);
            return Ok(ApiResponse.GetSuccessResponse(vm));
        }
    }
}
