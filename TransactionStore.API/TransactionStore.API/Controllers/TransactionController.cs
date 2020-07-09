using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
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

        private Mapper _mapper;
        public TransactionController(ILogger<TransactionController> logger)
        {
            _logger = logger;
            _mapper = new Mapper();
        }

        [HttpPost("deposit")]
        public ActionResult<long> CreateDepositTransaction([FromBody]TransactionInputModel transactionModel)
        {
            if (transactionModel.Amount <= 0) return BadRequest("The amount is missing");
            TransactionDto transactionDto = _mapper.ConvertTransactionInputModelDepositToTransactionDto(transactionModel);
            TransactionRepository repo = new TransactionRepository();
            return Ok(repo.Add(transactionDto));
        }
        
        [HttpPost("withdraw")] // прописать BadRequests
        public ActionResult<long> CreateWithdrawTransaction([FromBody]TransactionInputModel transactionModel)
        {            
            TransactionRepository repo = new TransactionRepository();
            TransactionDto transactionDto = _mapper.ConvertTransactionInputModelWithdrawToTransactionDto(transactionModel);
            return repo.Add(transactionDto);
        }
        
        [HttpPost("transfer")] // прописать BadRequests
        public ActionResult<List<long>> CreateTransferTransaction([FromBody]TransferInputModel transactionModel)
        {
            TransferTransactionDto transfer = _mapper.ConvertTransferInputModelToTransferTransactionDto(transactionModel);
            TransactionRepository repo = new TransactionRepository();
            return repo.AddTransfer(transfer);
        }

        [HttpGet("by-lead-id/{leadId}")] // прописать BadRequests
        public ActionResult<List<TransactionOutputModel>> GetTransactionsByLeadId(long leadId)
        {
            TransactionRepository repo = new TransactionRepository();
            return Ok(_mapper.ConvertTransactionDtoToTransactionOutputModels(repo.GetByLeadId(leadId)));
        }
        
        [HttpGet("{Id}")] // прописать BadRequests
        public ActionResult<List<TransactionOutputModel>> GetTransactionsById(long id)
        {
            TransactionRepository repo = new TransactionRepository();
            return Ok(_mapper.ConvertTransactionDtoToTransactionOutputModels(repo.GetById(id)));
        }
    }
}