﻿using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using TransactionStore.API.Models.Input;
using TransactionStore.API.Models.Output;
using TransactionStore.Data.DTO;
using TransactionStore.Data;
using TransactionStore.Core.Shared;
using TransactionStore.Business;
using Microsoft.AspNetCore.Http;
using AutoMapper;
using NLog;
using System.Threading.Tasks;

namespace TransactionStore.API.Controllers
{
    [ApiController]
    [Route("[Controller]")]
    public class TransactionController : Controller
    {    
        private readonly IMapper _mapper;
        private readonly ITransactionRepository _repo;
        private readonly ITransactionService _transactionService;
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public TransactionController(ITransactionRepository repo, ITransactionService transactionService, IMapper mapper)
        {        
            _repo = repo;
            _transactionService = transactionService;
            _mapper = mapper;           
        }

        private async ValueTask<string> FormBadRequest(decimal amount, long accountId)
        {
            if (amount <= 0)
            { 
                _logger.Info($"The amount is missing for AccountId [{accountId}]");
                return "The amount is missing";
            }
            var balance = await  _repo.GetBalanceByAccountId(accountId);
            if (balance.Data.Balance < 0) 
            {
                _logger.Info($"The balance of minus for AccountId [{accountId}]");
                return "The balance of minus"; 
            }
            if (balance.Data.Balance < amount) 
            {
                _logger.Info($"Not enough money for AccountId [{accountId}]");
                return "Not enough money"; 
            }
            return "";
        }

        /// <summary>
        /// Creates deposit operation and returns transaction's id :D
        /// </summary>
        /// <param name="transactionModel"></param>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost("deposit")]
        public async ValueTask<ActionResult<long>> CreateDepositTransaction([FromBody] TransactionInputModel transactionModel)
        {
            if (await _repo.GetByAccountId(transactionModel.AccountId) is null)
            {
                _logger.Info($"The Account [{transactionModel.AccountId}] is not found");
                return BadRequest("The account is not found");
            }
            if (transactionModel.Amount <= 0)
            {
                _logger.Info($"The amount is missing for AccountId [{transactionModel.AccountId}]");
                return BadRequest("The amount is missing");
            }
            var transactionDto = _mapper.Map<TransactionDto>(transactionModel);               
            var dataWrapper = await _transactionService.AddTransaction(1, transactionDto);
            _logger.Info($"Create Deposit Transaction with Amount = {transactionDto.Amount} {transactionDto.Currency} for AccountId [{transactionDto.AccountId}]");
            return MakeResponse(dataWrapper);
        }

        /// <summary>
        /// Creates withdraw operation and returns transaction's id ;D
        /// </summary>
        /// <param name="transactionModel"></param>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost("withdraw")]
        public async ValueTask<ActionResult<long>> CreateWithdrawTransaction([FromBody] TransactionInputModel transactionModel)
        {
            if (await _repo.GetByAccountId(transactionModel.AccountId) is null) return BadRequest("The account is not found");
            string badRequest = await FormBadRequest(transactionModel.Amount, transactionModel.AccountId);
            if (!string.IsNullOrWhiteSpace(badRequest)) return Problem("Not enough money on the account", statusCode: 418);
            var transactionDto = _mapper.Map<TransactionDto>(transactionModel);
            var dataWrapper = await _transactionService.AddTransaction(2, transactionDto);
            _logger.Info($"Create Withdraw Transaction with Amount = {transactionDto.Amount} {transactionDto.Currency} for AccountId [{transactionDto.AccountId}]");
            return MakeResponse(dataWrapper);
        }

        /// <summary>
        /// Creates transfer operation and returns transaction's ids ;-)
        /// </summary>
        /// <param name="transactionModel"></param>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost("transfer")]
        public async ValueTask<ActionResult<List<long>>> CreateTransferTransaction([FromBody] TransferInputModel transactionModel)
        {
            if (await _repo.GetById(transactionModel.AccountId) is null) return BadRequest("The account is not found");
            if (transactionModel.CurrencyId <= 0) return BadRequest("The currency is missing");
            string badRequest = await FormBadRequest(transactionModel.Amount, transactionModel.AccountId);
            if (!string.IsNullOrWhiteSpace(badRequest)) return Problem("Not enough money on the account", statusCode: 418);
            var transfer = _mapper.Map<TransferTransactionDto>(transactionModel);                
            var dataWrapper = await _repo.AddTransfer(transfer);
            _logger.Info($"Create Transfer Transaction with Amount = {transfer.Amount} {transfer.Currency} from AccountId [{transfer.AccountId}] for AccountId [{transfer.AccountIdReceiver}]");
            return MakeResponse(dataWrapper);
        }

        /// <summary>
        /// Gets list of transactions by accountId =)
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet("by-account-id/{accountId}")]
        public async ValueTask<ActionResult<List<TransactionOutputModel>>> GetTransactionsByAccountId(long accountId)
        {
            if (accountId <= 0) return BadRequest("Account was not found");
            var dataWrapper = await _transactionService.GetByAccountId(accountId);
            return MakeResponse(dataWrapper, _mapper.Map<List<TransactionOutputModel>>);
        }

        /// <summary>
        /// Gets list of transactions by Id =D
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet("{Id}")]
        public async ValueTask<ActionResult<List<TransactionOutputModel>>> GetTransactionById(long id)
        {
            if (id <= 0) return BadRequest("Transactions were not found");
            var dataWrapper = await _transactionService.GetById(id);
            return MakeResponse(dataWrapper, _mapper.Map<List<TransactionOutputModel>>);
        }

        /// <summary>
        /// Get account's balance by accountId in concrete currency |:-D
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet("{accountId}/balance")]
        public async ValueTask<ActionResult<BalanceOutputModel>> GetBalanceByAccountId(long accountId)
        {
            if (accountId <= 0) return BadRequest("Account was not found");
            var dataWrapper = await _repo.GetBalanceByAccountId(accountId);
            return MakeResponse(dataWrapper, _mapper.Map<BalanceOutputModel>);
        }

        /// <summary>
        /// Searches transactions by any parameters ^_^
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status200OK)]        
        [HttpPost("search")]
        public async ValueTask<ActionResult<List<TransactionOutputModel>>> SearchTransactionsByParameters([FromBody] SearchParametersInputModel searchModel)
        {
            if (string.IsNullOrEmpty(searchModel.FromDate)) searchModel.FromDate = null;            
            if (string.IsNullOrEmpty(searchModel.TillDate)) searchModel.TillDate = null;            
            if (searchModel.Type == (byte)TransactionType.Withdraw) searchModel.AmountBegin *= -1; 
            if (searchModel.Type == (byte)TransactionType.Withdraw) searchModel.AmountEnd *= -1; 
            var dataWrapper = await _transactionService.SearchTransactions(_mapper.Map<TransactionSearchParameters>(searchModel));
            return MakeResponse(dataWrapper, _mapper.Map<List<TransactionOutputModel>>);
        }

        /// <summary>
        /// Deletes all transactions 
        /// </summary>                   
        [HttpDelete]
        public async ValueTask DeleteAllTransactions()
        {
            await _repo.DeleteAllTransactions();
        }

        private delegate T DtoConverter<T, K>(K dto);

        private ActionResult<T> MakeResponse<T>(DataWrapper<T> dataWrapper)
        {
            if (!dataWrapper.IsOk)
            {
                return BadRequest(dataWrapper.ExceptionMessage);
            }
            return Ok(dataWrapper.Data);
        }

        private ActionResult<T> MakeResponse<T, K>(DataWrapper<K> dataWrapper, DtoConverter<T, K> dtoConverter)
        {
            if (!dataWrapper.IsOk)
            {
                return BadRequest(dataWrapper.ExceptionMessage);
            }
            return Ok(dtoConverter(dataWrapper.Data));
        }
    }
}