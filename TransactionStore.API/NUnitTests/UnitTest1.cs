using Microsoft.AspNetCore.Hosting;
using NUnit.Framework;
using System.Collections.Generic;
using TransactionStore.API.Models.Input;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework.Internal;
using System;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.TestHost;
using TransactionStore.API;
using TransactionStore.API.Models.Output;

namespace NUnitTests
{
    public class Tests
    {        
        IWebHostBuilder webHostBuilder;
        TestServer server;
        HttpClient client;

        [OneTimeSetUp]
        public void Setup()
        {
            webHostBuilder =
                  new WebHostBuilder()
                        .UseEnvironment("Development") // You can set the environment you want (development, staging, production)
                        .ConfigureServices(services => services.AddAutofac())
                        .UseStartup<Startup>(); // Startup class of your web app project


            server = new TestServer(webHostBuilder);
            client = server.CreateClient();
        }

        [Test]
        public async Task CreateDepositTest()
        {
            var transactionInputModel = new TransactionInputModel()
            {
                LeadId = 256,
                CurrencyId = 2,
                Amount = 80
            };
            var jsonContent = new StringContent(JsonConvert.SerializeObject(transactionInputModel), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("https://localhost:44388/transaction/deposit", jsonContent);
            long id = Convert.ToInt64(await response.Content.ReadAsStringAsync());

            string result = await client.GetStringAsync($"https://localhost:44388/transaction/{id}");
            var actual = JsonConvert.DeserializeObject<List<TransactionOutputModel>>(result)[0];

            Assert.AreEqual(actual.LeadId, 256);
            Assert.AreEqual(actual.Currency, "USD");
            Assert.AreEqual(actual.Amount, 80);
            Assert.AreEqual(actual.Type, "Deposit");
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            server.Dispose();
            client.Dispose();
        }
    }
}