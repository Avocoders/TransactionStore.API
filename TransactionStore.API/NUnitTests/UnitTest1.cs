using Microsoft.AspNetCore.Hosting;
using NUnit.Framework;
using System.Collections.Generic;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework.Internal;
using System;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.TestHost;
using TransactionStore.API;
using TransactionStore.API.Models.Input;
using TransactionStore.API.Models.Output;
using System.Text.RegularExpressions;

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
                AccountId = 256,
                CurrencyId = 2,
                Amount = 80
            };
            var jsonContent = new StringContent(JsonConvert.SerializeObject(transactionInputModel), Encoding.UTF8, "application/json");
            var response = await client.PostAsync(Configuration.LocalHost + "transaction/deposit", jsonContent);
            long id = Convert.ToInt64(await response.Content.ReadAsStringAsync());

            string result = await client.GetStringAsync(Configuration.LocalHost + $"transaction/{id}");
            var actual = JsonConvert.DeserializeObject<List<TransactionOutputModel>>(result)[0];

            Assert.AreEqual(actual.AccountId, 256);
            Assert.AreEqual(actual.Currency, "USD");
            Assert.AreEqual(actual.Amount, 80);
            Assert.AreEqual(actual.Type, "Deposit");
        }

        [Test]
        public async Task CreateWithdrawTest()
        {
            var transactionInputModel = new TransactionInputModel()
            {
                AccountId = 256,
                CurrencyId = 1,
                Amount = 10
            };
            var jsonContent = new StringContent(JsonConvert.SerializeObject(transactionInputModel), Encoding.UTF8, "application/json");
            var response = await client.PostAsync(Configuration.LocalHost + "transaction/withdraw", jsonContent);
            long id = Convert.ToInt64(await response.Content.ReadAsStringAsync());

            string result = await client.GetStringAsync(Configuration.LocalHost + $"transaction/{id}");
            var actual = JsonConvert.DeserializeObject<List<TransactionOutputModel>>(result)[0];

            Assert.AreEqual(actual.AccountId, 256);
            Assert.AreEqual(actual.Currency, "RUR");
            Assert.AreEqual(actual.Amount, -10);
            Assert.AreEqual(actual.Type, "Withdraw");
        }

        [Test]
        public async Task CreateTransferTest()
        {
            var transferInputModel = new TransferInputModel()
            {
                AccountId = 256,
                CurrencyId = 2,
                Amount = 80,
                AccountIdReceiver = 257
            };
            var jsonContent = new StringContent(JsonConvert.SerializeObject(transferInputModel), Encoding.UTF8, "application/json");
            var response = await client.PostAsync(Configuration.LocalHost + "transaction/transfer", jsonContent);
            string ids = Convert.ToString(await response.Content.ReadAsStringAsync());
            string[] data = Regex.Split(ids, @"\D+");
            long id = Convert.ToInt64(data[1]);
            string result = await client.GetStringAsync(Configuration.LocalHost + $"transaction/{id}");
            var actual = JsonConvert.DeserializeObject<List<TransactionOutputModel>>(result)[0];

            Assert.AreEqual(actual.AccountId, 256);
            Assert.AreEqual(actual.Currency, "USD");
            Assert.AreEqual(actual.Amount, -80);
            Assert.AreEqual(actual.Type, "Transfer");
            Assert.AreEqual(actual.AccountIdReceiver, 257);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            server.Dispose();
            client.Dispose();
        }

    }
}