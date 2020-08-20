using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TransactionStore.API.Models.Input
{
    public class PaypalInputModel
    {              
            public string Intent { get; set; }
            public PayerInputModel Payer { get; set; }
            public string  Payment_method { get; set; }

            public List<TransactionsInputModel> Transactions { get; set; }
            
            public Redirect_Urls Redirect_Url { get; set; }
        
     }

}
