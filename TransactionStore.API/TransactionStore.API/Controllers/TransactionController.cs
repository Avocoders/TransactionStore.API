using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TransactionStore.API.Models.Input;
using TransactionStore.API.Models.Output;
using TransactionStore.Data.DTO;
using TransactionStore.Data;
using TransactionStore.Core.Shared;
using System;
using TransactionStore.Business;
using Microsoft.AspNetCore.Http;

namespace TransactionStore.API.Controllers
{
    [ApiController]
    [Route("[Controller]")]
    public class TransactionController : Controller
    {
        private readonly ILogger<TransactionController> _logger;
        private readonly Mapper _mapper;
        private readonly ITransactionRepository _repo;
        private readonly ITransactionService _transactionService;
        public TransactionController(ILogger<TransactionController> logger, ITransactionRepository repo, ITransactionService transactionService)
        {
            _logger = logger;
            _mapper = new Mapper();
            _repo = repo;
            _transactionService = transactionService;
        }

        private string FormBadRequest(decimal amount, long leadId, byte currencyId)
        {
            if (amount <= 0) return "The amount is missing";
            decimal balance = _repo.GetTotalAmountInCurrency(leadId, currencyId);
            if (balance < 0) return "The balance of minus";
            if (balance < amount) return "Not enough money";
            return "";
        }

        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost("deposit")]
        public ActionResult<long> CreateDepositTransaction([FromBody] TransactionInputModel transactionModel)
        {
            if (_repo.GetById(transactionModel.LeadId) is null) return BadRequest("The user is not found");
            if (transactionModel.CurrencyId <= 0) return BadRequest("The currency is missing");
            string badRequest = FormBadRequest(transactionModel.Amount, transactionModel.LeadId, transactionModel.CurrencyId);
            if (!string.IsNullOrWhiteSpace(badRequest)) return BadRequest(badRequest);
            TransactionDto transactionDto = _mapper.ConvertTransactionInputModelDepositToTransactionDto(transactionModel);
            DataWrapper<long> dataWrapper = _repo.Add(transactionDto);
            return MakeResponse(dataWrapper);
        }

        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost("withdraw")]
        public ActionResult<long> CreateWithdrawTransaction([FromBody] TransactionInputModel transactionModel)
        {
            if (_repo.GetById(transactionModel.LeadId) is null) return BadRequest("The user is not found");
            if (transactionModel.CurrencyId <= 0) return BadRequest("The currency is missing");
            string badRequest = FormBadRequest(transactionModel.Amount, transactionModel.LeadId, transactionModel.CurrencyId);
            if (!string.IsNullOrWhiteSpace(badRequest)) return BadRequest(badRequest);
            TransactionDto transactionDto = _mapper.ConvertTransactionInputModelWithdrawToTransactionDto(transactionModel);
            DataWrapper<long> dataWrapper = _repo.Add(transactionDto);
            return MakeResponse(dataWrapper);
        }

        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost("transfer")]
        public ActionResult<List<long>> CreateTransferTransaction([FromBody] TransferInputModel transactionModel)
        {
            if (_repo.GetById(transactionModel.LeadId) is null) return BadRequest("The user is not found");
            if (transactionModel.CurrencyId <= 0) return BadRequest("The currency is missing");
            string badRequest = FormBadRequest(transactionModel.Amount, transactionModel.LeadId, transactionModel.CurrencyId);
            if (!string.IsNullOrWhiteSpace(badRequest)) return BadRequest(badRequest);
            TransferTransaction transfer = _mapper.ConvertTransferInputModelToTransferTransaction(transactionModel);
            DataWrapper<List<long>> dataWrapper = _repo.AddTransfer(transfer);
            return MakeResponse(dataWrapper);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet("by-lead-id/{leadId}")]
        public ActionResult<List<TransactionOutputModel>> GetTransactionsByLeadId(long leadId)
        {
            if (leadId <= 0) return BadRequest("Lead was not found");
            DataWrapper<List<TransactionDto>> dataWrapper = _transactionService.GetByLeadId(leadId);
            return MakeResponse(dataWrapper, _mapper.ConvertTransactionDtosToTransactionOutputModels);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet("{Id}")]
        public ActionResult<List<TransactionOutputModel>> GetTransactionById(long id)
        {
            if (id <= 0) return BadRequest("Transactions were not found");
            DataWrapper<List<TransactionDto>> dataWrapper = _transactionService.GetById(id);
            return MakeResponse(dataWrapper, _mapper.ConvertTransactionDtosToTransactionOutputModels);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet("{leadId}/balance/{currencyId}")]
        public ActionResult<decimal> GetBalanceByLeadIdInCurrency(long leadId, byte currencyId)
        {
            if (leadId <= 0) return BadRequest("Lead was not found");
            if (currencyId <= 0) return BadRequest("Currency was not found");
            return _repo.GetTotalAmountInCurrency(leadId, currencyId);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]        
        [HttpPost("search")]
        public ActionResult<List<TransactionOutputModel>> GetTransactionSearchParameters([FromBody] SearchParametersInputModel searchModel)
        {
            var dataWrapper = _transactionService.SearchTransactions(_mapper.ConvertSearchParametersInputModelToTransactionSearchParameters(searchModel));
            return MakeResponse(dataWrapper, _mapper.ConvertTransactionDtosToTransactionOutputModels);
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