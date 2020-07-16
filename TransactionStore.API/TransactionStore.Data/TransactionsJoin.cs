using System;
using System.Collections.Generic;
using System.Text;
using System.Transactions;
using TransactionStore.Data.DTO;

namespace TransactionStore.Data
{
    public class TransactionsJoin<N,T>
    {
        public N NonTransfers { get; set; }
        public T Transfers { get; set; }
        public bool IsOk { get; set; } = false;
        public string ExceptionMessage { get; set; }
    }
}
