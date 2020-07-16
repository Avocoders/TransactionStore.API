﻿using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TransactionStore.API.Models.Input;
using TransactionStore.API.Models.Output;
using TransactionStore.Data.DTO;
using TransactionStore.Data;

namespace TransactionStore.API.Controllers
{
    [ApiController]
    [Route("[Controller]")]
    public class TransactionController : Controller
    {
        private readonly ILogger<TransactionController> _logger;
        private readonly Mapper _mapper;
        private readonly TransactionRepository _repo;
        public TransactionController(ILogger<TransactionController> logger)
        {
            _logger = logger;
            _mapper = new Mapper();
            _repo = new TransactionRepository();
        }

        private string FormBadRequest(decimal amount, long leadId, byte currencyId)
        {
            if (amount <= 0) return "The amount is missing";
            decimal balance = _repo.GetTotalAmountInCurrency(leadId, currencyId);
            if (balance < 0) return "The balance of minus";
            if (balance < amount) return "Not enough money";
            return "";
        }

        [HttpPost("deposit")]
        public ActionResult<long> CreateDepositTransaction([FromBody]TransactionInputModel transactionModel)
        {
            if (transactionModel.Amount <= 0) return BadRequest("The amount is missing");
            TransactionDto transactionDto = _mapper.ConvertTransactionInputModelDepositToTransactionDto(transactionModel);
            DataWrapper<long> dataWrapper = _repo.Add(transactionDto);
            return MakeResponse(dataWrapper);
        }
        
        [HttpPost("withdraw")]
        public ActionResult<long> CreateWithdrawTransaction([FromBody]TransactionInputModel transactionModel)
        {
            string badRequest = FormBadRequest(transactionModel.Amount, transactionModel.LeadId, transactionModel.CurrencyId);
            if (!string.IsNullOrWhiteSpace(badRequest)) return BadRequest(badRequest);            
            TransactionDto transactionDto = _mapper.ConvertTransactionInputModelWithdrawToTransactionDto(transactionModel);
            DataWrapper<long> dataWrapper = _repo.Add(transactionDto);
            return MakeResponse(dataWrapper);
        }
        
        [HttpPost("transfer")]
        public ActionResult<List<long>> CreateTransferTransaction([FromBody]TransferInputModel transactionModel)
        {
            string badRequest = FormBadRequest(transactionModel.Amount, transactionModel.LeadId, transactionModel.CurrencyId);
            if (!string.IsNullOrWhiteSpace(badRequest)) return BadRequest(badRequest);            
            TransferTransaction transfer = _mapper.ConvertTransferInputModelToTransferTransactionDto(transactionModel);
            DataWrapper<List<long>> dataWrapper = _repo.AddTransfer(transfer);
            return MakeResponse(dataWrapper);
        }

        [HttpGet("by-lead-id/{leadId}")]
        public ActionResult<List<TransactionOutputModel>> GetTransactionsByLeadId(long leadId)
        {
            DataWrapper<List<TransferTransaction>> dataWrapper = _repo.GetByLeadId(leadId);
            return MakeResponse(dataWrapper, _mapper.ConvertTransferTransactionsToTransactionOutputModel);
        }
        
        [HttpGet("{Id}")]
        public ActionResult<TransactionOutputModel> GetTransactionById(long id)
        {
            DataWrapper<TransferTransaction> dataWrapper = _repo.GetById(id);
            return MakeResponse(dataWrapper, _mapper.ConvertTransferTransactionToTransactionOutputModel);
        }

        [HttpGet("{leadId}/balance/{currencyId}")]
        public ActionResult<decimal> GetBalanceByLeadIdInCurrency(long leadId, byte currencyId)
        {
            return _repo.GetTotalAmountInCurrency(leadId, currencyId);
        }

        [HttpGet("search")]
        public ActionResult<List<TransactionOutputModel>> GetTransactionSearchParameters(SearchParametersInputModel searchModel)
        {
            DataWrapper<List<TransactionDto>> dataWrapper;
            dataWrapper = _repo.SearchTransactions(_mapper.ConvertSearchParametersInputModelToTransactionSearchParameters(searchModel));
            if (!dataWrapper.IsOk) return BadRequest(dataWrapper.ExceptionMessage);
            return dataWrapper;
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