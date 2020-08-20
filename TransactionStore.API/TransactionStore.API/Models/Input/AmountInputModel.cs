using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TransactionStore.API.Models.Input
{
    public class AmountInputModel
    {
         public string Total { get; set; }
         public string Currency { get; set; }
    }
}
