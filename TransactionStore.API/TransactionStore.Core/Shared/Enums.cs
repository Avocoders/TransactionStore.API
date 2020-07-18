﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TransactionStore.Core.Shared
{
    public enum TransactionType
    {
        Deposit = 1,
        Withdraw,
        Transfer
    }

    public enum TransactionCurrency
    {
        RUR = 1,
        USD,
        EUR
    }
}