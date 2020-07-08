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

        private Mapper mapper;
        public TransactionController(ILogger<TransactionController> logger)
        {
            _logger = logger;
            mapper = new Mapper();
        }

        [HttpPost("deposit")] // прописать BadRequests
        public ActionResult<long> CreateDepositTransaction([FromBody]TransactionInputModel transactionModel)
        {
            TransactionDto transactionDto = mapper.ConvertTransactionInputModelDepositToTransactionDto(transactionModel);
            TransactionRepository transaction = new TransactionRepository();
            return transaction.Add(transactionDto);
        }
        
        [HttpPost("withdraw")] // прописать BadRequests
        public ActionResult<long> CreateWithdrawTransaction([FromBody]TransactionInputModel transactionModel)
        {
            // менять значение на отрицательное в mapper
            TransactionDto transactionDto = mapper.ConvertTransactionInputModelWithdrawToTransactionDto(transactionModel);
            TransactionRepository transaction = new TransactionRepository();
            return transaction.Add(transactionDto);
        }
        
        [HttpPost("transfer")] // прописать BadRequests
        public ActionResult<long> CreateTransferTransaction([FromBody]TransferInputModel transactionModel) // добавить DestinationLeadId
        {
            TransactionDto transactionDto = mapper.ConvertTransactionInputModelTransferToTransactionDto(transactionModel);
            TransactionRepository transaction = new TransactionRepository();
            return transaction.Add(transactionDto);
        }

        [HttpGet("by-lead-id/{leadId}")] // прописать BadRequests
        public ActionResult<List<TransactionOutputModel>> GetTransactionsByLeadId(long leadId)
        {
            TransactionRepository transaction = new TransactionRepository();
            return Ok(mapper.ConvertTransactionDtoToTransactionOutputModels(transaction.GetByLeadId(leadId)));
        }
        
        [HttpGet("{Id}")] // прописать BadRequests
        public ActionResult<List<TransactionOutputModel>> GetTransactionsById(long id)
        {
            TransactionRepository transaction = new TransactionRepository();
            return Ok(mapper.ConvertTransactionDtoToTransactionOutputModels(transaction.GetById(id)));
        }
    }
}