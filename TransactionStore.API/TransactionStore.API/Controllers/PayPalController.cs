using System.Collections.Generic;
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
using Messaging;
using RestSharp;
using TransactionStore.Core;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace TransactionStore.API.Controllers
{ 
   
    [ApiController]
    [Route("[Controller]")]
    public class PayPalController : Controller
    {
        private readonly IMapper _mapper;
        private readonly RestClient _restClient;
        private readonly ITransactionRepository _repo;
        private readonly ITransactionService _transactionService;
        private Currencies _currencies;
        public PayPalController(ITransactionRepository repo, ITransactionService transactionService, IOptions<UrlOptions> options, IMapper mapper, Currencies currencies)
        {
            _repo = repo;
            _transactionService = transactionService;
            _mapper = mapper;
            _currencies = currencies;
            _restClient = new RestClient(options.Value.PayPalUrl);
        }


        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost("payments/payment")]
        public async Task<ActionResult<string>> CreateDepositTransaction([FromBody] PaypalInputModel paypalInputModel)
        {           
            var restRequest = new RestRequest("payments/payment", Method.POST, DataFormat.Json);
            restRequest.AddJsonBody(paypalInputModel);
            return _restClient.Execute<string>(restRequest).Data; ;
        }

    }
}