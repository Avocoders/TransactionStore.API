using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using AutoMapper;
using System.Net.Http;
using System.Threading.Tasks;

namespace TransactionStore.API.Controllers
{
    [ApiController]
    [Route("[Controller]")]
    public class RatesController : Controller
    {
        private readonly ILogger<TransactionController> _logger;        
        private readonly IMapper _mapper;
        private readonly HttpClient _httpClient;
        private string _currencyRates;

        public RatesController(ILogger<TransactionController> logger, IMapper mapper)
        {
            _logger = logger;            
            _mapper = mapper;
            _httpClient = new HttpClient();
        }               
        
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet]
              
        public async Task<string> ReceiveCurrencyRates()
        {
            var response = await _httpClient.GetAsync($"https://localhost:44352/rates");
            return _currencyRates = await response.Content.ReadAsStringAsync();            
        }
    }
}