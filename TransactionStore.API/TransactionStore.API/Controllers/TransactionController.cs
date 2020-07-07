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
            TransactionDTO transactionDTO = mapper.ConvertTransactionInputModelToTransactionDTO(transactionModel);
            TransactionCRUD transactionCRUD = new TransactionCRUD();
            return transactionCRUD.Add(transactionDTO);
        }

        [HttpGet("{leadId}")]
        public ActionResult<List<TransactionOutputModel>> GetTransactionsByLeadId(long leadId)
        {
            Mapper mapper = new Mapper();
            TransactionCRUD transaction = new TransactionCRUD();
            return Ok(mapper.ConvertTransactionDTOsToTransactionOutputModels(transaction.GetByLeadId(leadId)));
        }

    }
}