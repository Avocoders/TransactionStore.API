using Microsoft.AspNetCore.Hosting;
using TransactionStore.API;

namespace NUnitTests
{
    public class TestStartup : Startup
    {
        public TestStartup(IWebHostEnvironment testenv) : base(testenv)
        {

        }
    }
}