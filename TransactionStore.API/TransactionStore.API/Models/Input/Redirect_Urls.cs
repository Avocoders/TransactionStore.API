using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TransactionStore.API.Models.Input
{
    public class Redirect_Urls
    {
        public string Return_url { get; set; } = "https://sandbox.paypal.com";
        public string Cancel_url { get; set; } = "https://sandbox.paypal.com";
    }
}
