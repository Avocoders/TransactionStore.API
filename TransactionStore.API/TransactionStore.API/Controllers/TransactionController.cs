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
        
        [HttpPost("withdraw")]
        public ActionResult<long> CreateWithdrawTransaction([FromBody]TransactionInputModel transactionModel)
        {
            if (transactionModel.Amount <= 0) return BadRequest("The amount is missing");
            TransactionRepository repo = new TransactionRepository();
            if (repo.GetTotalAmount(transactionModel.LeadId)<0) return BadRequest("The total amount of minus");
            if (repo.GetTotalAmount(transactionModel.LeadId) < transactionModel.Amount) return BadRequest("Not enough money");
            TransactionDto transactionDto = _mapper.ConvertTransactionInputModelWithdrawToTransactionDto(transactionModel);
            return Ok(repo.Add(transactionDto));
        }
        
        [HttpPost("transfer")]
        public ActionResult<List<long>> CreateTransferTransaction([FromBody]TransferInputModel transactionModel)
        {
            if (transactionModel.Amount <= 0) return BadRequest("The amount is missing");
            TransactionRepository repo = new TransactionRepository();
            if (repo.GetTotalAmount(transactionModel.LeadId) < 0) return BadRequest("The total amount of minus");
            if (repo.GetTotalAmount(transactionModel.LeadId)<transactionModel.Amount) return BadRequest("Not enough money");
            TransferTransactionDto transfer = _mapper.ConvertTransferInputModelToTransferTransactionDto(transactionModel);
            return Ok(repo.AddTransfer(transfer));
        }

        [HttpGet("by-lead-id/{leadId}")]
        public ActionResult<List<TransactionOutputModel>> GetTransactionsByLeadId(long leadId)
        {
            TransactionRepository repo = new TransactionRepository();
            return Ok(_mapper.ConvertTransactionDtoToTransactionOutputModels(repo.GetByLeadId(leadId)));
        }
        
        [HttpGet("{Id}")]
        public ActionResult<TransactionOutputModel> GetTransactionById(long id)
        {
            TransactionRepository repo = new TransactionRepository();
            return Ok(_mapper.ConvertTransactionDtoToTransactionOutputModel(repo.GetById(id)));
        }
    }
}