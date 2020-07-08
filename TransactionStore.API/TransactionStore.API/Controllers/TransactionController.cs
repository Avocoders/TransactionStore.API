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
using TransactionStore.Data.StoredProcedure;

namespace TransactionStore.API.Controllers
{
    [ApiController]
    [Route("[Controller]")]
    public class TransactionController : Controller
    {
        private readonly ILogger<TransactionController> _logger;

        public TransactionController(ILogger<TransactionController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public ActionResult<long> PostTransaction(TransactionInputModel transactionModel)
        {
            Mapper mapper = new Mapper();
            TransactionDto transactionDto = mapper.ConvertTransactionInputModelToTransactionDto(transactionModel);
            TransactionCrud transaction = new TransactionCrud();
            return transaction.Add(transactionDto);
        }

        [HttpGet("lead/{leadId}")]
        public ActionResult<List<TransactionOutputModel>> GetTransactionsByLeadId(long leadId)
        {
            Mapper mapper = new Mapper();
            TransactionCrud transaction = new TransactionCrud();
            return Ok(mapper.ConvertTransactionDtoToTransactionOutputModels(transaction.GetByLeadId(leadId)));
        }
        
        [HttpGet("{Id}")]
        public ActionResult<List<TransactionOutputModel>> GetTransactionsById(long id)
        {
            Mapper mapper = new Mapper();
            TransactionCrud transaction = new TransactionCrud();
            return Ok(mapper.ConvertTransactionDtoToTransactionOutputModels(transaction.GetById(id)));
        }
    }
}