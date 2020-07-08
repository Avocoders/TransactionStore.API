using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TransactionStore.API.Models.Input
{
    public class TransactionInputModel
    {
        public long LeadId { get; set; }
        public byte CurrencyId { get; set; }
        public decimal Amount { get; set; }
    }
}
