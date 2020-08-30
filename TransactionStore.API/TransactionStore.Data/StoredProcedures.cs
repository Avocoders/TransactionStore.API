using System;
using System.Collections.Generic;
using System.Text;

namespace TransactionStore.Data
{
    public static class StoredProcedures
    {
        public const string AddTransaction = "Transaction_Add";
        public const string AddTranfer = "Transaction_AddTransfer";
        public const string GetTransactionById = "Transaction_GetById";
        public const string GetTransactionByAccountId = "Transaction_GetByAccountId";
        public const string SearchTransaction = "Transaction_Search";
        public const string GetBalanceByAccountId = "Transaction_GetBalanceByAccountId";
        public const string UpdateCurrencyRates = "CurrencyRates_Update";
        public const string GetCurrencyRateById = "CurrencyRates_GetById";
        public const string DeleteAll = "Transaction_DeleteAll";
    }
}
