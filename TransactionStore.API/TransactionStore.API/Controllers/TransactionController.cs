﻿using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TransactionStore.API.Models.Input;
using TransactionStore.API.Models.Output;
using TransactionStore.Data.DTO;
using TransactionStore.Data;
using TransactionStore.Core.Shared;
using TransactionStore.Business;
using Microsoft.AspNetCore.Http;
using AutoMapper;

namespace TransactionStore.API.Controllers
{
    [ApiController]
    [Route("[Controller]")]
    public class TransactionController : Controller
    {
        private readonly ILogger<TransactionController> _logger;        
        private readonly IMapper _mapper;
        private readonly ITransactionRepository _repo;
        private readonly ITransactionService _transactionService;
        public TransactionController(ILogger<TransactionController> logger, ITransactionRepository repo, ITransactionService transactionService, IMapper mapper)
        {
            _logger = logger;            
            _repo = repo;
            _transactionService = transactionService;
            _mapper = mapper;
        }

        private string FormBadRequest(decimal amount, long leadId, byte currencyId)
        {
            if (amount <= 0) return "The amount is missing";
            decimal balance = _repo.GetTotalAmountInCurrency(leadId, currencyId);
            if (balance < 0) return "The balance of minus";
            if (balance < amount) return "Not enough money";
            return "";
        }

        /// <summary>
        /// Creates deposit operation and returns transaction's id!!!:D
        /// </summary>
        /// <param name="transactionModel"></param>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost("deposit")]
        public ActionResult<long> CreateDepositTransaction([FromBody] TransactionInputModel transactionModel)
        {
            if (_repo.GetById(transactionModel.LeadId) is null) return BadRequest("The user is not found");
            if (transactionModel.CurrencyId <= 0) return BadRequest("The currency is missing");            
            TransactionDto transactionDto = _mapper.Map<TransactionDto>(transactionModel);
            transactionDto.Type = new TransactionTypeDto() { Id = (byte)TransactionType.Deposit};       //бизнес логика    
            DataWrapper<long> dataWrapper = _repo.Add(transactionDto);
            return MakeResponse(dataWrapper);
        }

        /// <summary>
        /// Creates withdraw operation and returns transaction's id!!!;D
        /// </summary>
        /// <param name="transactionModel"></param>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost("withdraw")]
        public ActionResult<long> CreateWithdrawTransaction([FromBody] TransactionInputModel transactionModel)
        {
            if (_repo.GetById(transactionModel.LeadId) is null) return BadRequest("The user is not found");
            if (transactionModel.CurrencyId <= 0) return BadRequest("The currency is missing");
            string badRequest = FormBadRequest(transactionModel.Amount, transactionModel.LeadId, transactionModel.CurrencyId);
            if (!string.IsNullOrWhiteSpace(badRequest)) return BadRequest(badRequest);
            TransactionDto transactionDto = _mapper.Map<TransactionDto>(transactionModel);
            transactionDto.Type = new TransactionTypeDto() { Id = (byte)TransactionType.Withdraw };   //  бизнес логика       
            transactionDto.Amount *= -1;  //бизнес логика
            DataWrapper<long> dataWrapper = _repo.Add(transactionDto);
            return MakeResponse(dataWrapper);
        }

        /// <summary>
        /// Creates transfer operation and returns transaction's ids!!!;-)
        /// </summary>
        /// <param name="transactionModel"></param>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost("transfer")]
        public ActionResult<List<long>> CreateTransferTransaction([FromBody] TransferInputModel transactionModel)
        {
            if (_repo.GetById(transactionModel.LeadId) is null) return BadRequest("The user is not found");
            if (transactionModel.CurrencyId <= 0) return BadRequest("The currency is missing");
            string badRequest = FormBadRequest(transactionModel.Amount, transactionModel.LeadId, transactionModel.CurrencyId);
            if (!string.IsNullOrWhiteSpace(badRequest)) return BadRequest(badRequest);
            TransferTransaction transfer = _mapper.Map<TransferTransaction>(transactionModel);                
            DataWrapper<List<long>> dataWrapper = _repo.AddTransfer(transfer);
            return MakeResponse(dataWrapper);
        }

        /// <summary>
        /// Gets list of transactions by leadId =)
        /// </summary>
        /// <param name="leadId"></param>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet("by-lead-id/{leadId}")]
        public ActionResult<List<TransactionOutputModel>> GetTransactionsByLeadId(long leadId)
        {
            if (leadId <= 0) return BadRequest("Lead was not found");
            DataWrapper<List<TransactionDto>> dataWrapper = _transactionService.GetByLeadId(leadId);
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
        public ActionResult<List<TransactionOutputModel>> GetTransactionById(long id)
        {
            if (id <= 0) return BadRequest("Transactions were not found");
            DataWrapper<List<TransactionDto>> dataWrapper = _transactionService.GetById(id);
            return MakeResponse(dataWrapper, _mapper.Map<List<TransactionOutputModel>>);
        }

        /// <summary>
        /// Get lead's balance by leadId in concrete currency |:-D
        /// </summary>
        /// <param name="leadId"></param>
        /// <param name="currencyId"></param>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet("{leadId}/balance/{currencyId}")]
        public ActionResult<decimal> GetBalanceByLeadIdInCurrency(long leadId, byte currencyId)
        {
            if (leadId <= 0) return BadRequest("Lead was not found");
            if (currencyId <= 0) return BadRequest("Currency was not found");
            return _repo.GetTotalAmountInCurrency(leadId, currencyId);
        }

        /// <summary>
        /// Searches transactions by any parameters ^_^
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status200OK)]        
        [HttpPost("search")]
        public ActionResult<List<TransactionOutputModel>> GetTransactionSearchParameters([FromBody] SearchParametersInputModel searchModel)
        {
            if (string.IsNullOrEmpty(searchModel.FromDate)) searchModel.FromDate = null;            
            if (string.IsNullOrEmpty(searchModel.TillDate)) searchModel.TillDate = null;            
            if (searchModel.Type == (byte)TransactionType.Withdraw) searchModel.AmountBegin *= -1; 
            if (searchModel.Type == (byte)TransactionType.Withdraw) searchModel.AmountEnd *= -1; 
            var dataWrapper = _transactionService.SearchTransactions(_mapper.Map<TransactionSearchParameters>(searchModel));
            return MakeResponse(dataWrapper, _mapper.Map<List<TransactionOutputModel>>);
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