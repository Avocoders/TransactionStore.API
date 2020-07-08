using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TransactionStore.API.Shared
{
    public class Enums
    {
        public enum TransactionType
        {
            Deposit=1,
            Withdraw,
            Transfer
        }
    }
}
