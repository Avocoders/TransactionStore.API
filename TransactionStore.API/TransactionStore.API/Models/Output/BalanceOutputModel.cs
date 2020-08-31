using System;

namespace TransactionStore.API.Models.Output
{
    public class BalanceOutputModel
    {
        public decimal Balance { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
