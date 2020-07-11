using System.Collections.Generic;
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

        [HttpPost("deposit")]
        public ActionResult<long> CreateDepositTransaction([FromBody]TransactionInputModel transactionModel)
        {
            if (transactionModel.Amount <= 0) return BadRequest("The amount is missing");
            TransactionDto transactionDto = _mapper.ConvertTransactionInputModelDepositToTransactionDto(transactionModel);
            return Ok(_repo.Add(transactionDto));
        }
        
        [HttpPost("withdraw")]
        public ActionResult<long> CreateWithdrawTransaction([FromBody]TransactionInputModel transactionModel)
        {
            string badRequest = _repo.FormBadRequest(transactionModel.Amount, transactionModel.LeadId, transactionModel.CurrencyId);
            if (!string.IsNullOrWhiteSpace(badRequest)) return BadRequest(badRequest);            
            TransactionDto transactionDto = _mapper.ConvertTransactionInputModelWithdrawToTransactionDto(transactionModel);
            return Ok(_repo.Add(transactionDto));
        }
        
        [HttpPost("transfer")]
        public ActionResult<List<long>> CreateTransferTransaction([FromBody]TransferInputModel transactionModel)
        {
            string badRequest = _repo.FormBadRequest(transactionModel.Amount, transactionModel.LeadId, transactionModel.CurrencyId);
            if (!string.IsNullOrWhiteSpace(badRequest)) return BadRequest(badRequest);            
            TransferTransactionDto transfer = _mapper.ConvertTransferInputModelToTransferTransactionDto(transactionModel);
            return Ok(_repo.AddTransfer(transfer));
        }

        [HttpGet("by-lead-id/{leadId}")]
        public ActionResult<List<TransferOutputModel>> GetTransactionsByLeadId(long leadId)
        {
            return Ok(_mapper.ConvertTransferTransactionDtosToTransferOutputModels(_repo.GetByLeadId(leadId)));
        }
        
        [HttpGet("{Id}")]
        public ActionResult<TransactionOutputModel> GetTransactionById(long id)
        {
            return Ok(_mapper.ConvertTransferTransactionDtoToTransferOutputModel(_repo.GetById(id)));
        }

        [HttpGet("{leadId}/balance/{currencyId}")]
        public ActionResult<decimal> GetBalanceByLeadIdInCurrency(long leadId, byte currencyId)
        {
            return _repo.GetTotalAmountInCurrency(leadId, currencyId);
        }
    }
}