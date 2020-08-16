using System;
using System.Linq;
using System.Collections.Generic;
using System.Data;
using TransactionStore.Data.DTO;
using Dapper;
using TransactionStore.Core.Shared;
using System.Data.SqlClient;
using TransactionStore.Core;
using Microsoft.Extensions.Options;
using Messaging;

namespace TransactionStore.Data
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly IDbConnection _connection;
        private readonly Currencies _currencies;
        public TransactionRepository(IOptions<StorageOptions> options, Currencies currencies)
        {
            _connection = new SqlConnection(options.Value.DBConnectionString);
            _currencies = currencies;
        }

        public DataWrapper<long> Add(TransactionDto transaction) 
        {   
            var result = new DataWrapper<long>();
            try
            {
                string sqlExpression = "Transaction_Add @accountId, @typeId, @currencyId, @amount, @exchangeRates";
                result.Data = _connection.Query<long>(sqlExpression,
                    new
                    {
                        transaction.Id,
                        transaction.AccountId,
                        TypeId = transaction.Type.Id,
                        CurrencyId = transaction.Currency.Id,
                        transaction.Amount,
                        ExchangeRates = GetRates(transaction.Currency.Id.Value)

                    }).FirstOrDefault();
                result.IsOk = true;
            }

            catch (Exception e)
            {
                result.ExceptionMessage = e.Message;
            }
            return result;
        }

        public DataWrapper<List<long>> AddTransfer(TransferTransactionDto transfer)
        {
            var exchangeRates1 = GetRates(transfer.Currency.Id.Value);
            var exchangeRates2 = GetRates(transfer.ReceiverCurrencyId);

            var result = new DataWrapper<List<long>>();
            try
            {
                string sqlExpression = "Transaction_AddTransfer";
                result.Data = _connection.Query<long>(sqlExpression,
                    new
                    {
                        accountId = transfer.AccountId,
                        typeId = transfer.Type.Id,
                        currencyId = transfer.Currency.Id,
                        amount1 = transfer.Amount,
                        amount2 = transfer.Amount / exchangeRates1 * exchangeRates2,
                        accountIdReceiver = transfer.AccountIdReceiver,
                        receiverCurrencyId = transfer.ReceiverCurrencyId,
                        exchangeRates1,
                        exchangeRates2,

                    }, commandType:CommandType.StoredProcedure).ToList();
                result.IsOk = true;
            }
            catch (Exception e)
            {
                result.ExceptionMessage = e.Message;
            }
            return result;
        }

        public DataWrapper<List<TransactionDto>> GetById(long id)
        {
            var result = new DataWrapper<List<TransactionDto>>();
            try
            {
                string sqlExpression = "Transaction_GetById @id";
                result.Data = _connection.Query<TransactionDto, TransactionTypeDto, TransactionDto>(sqlExpression,
                    (transaction, type) =>
                    {
                        TransactionDto transactionEntry;
                        transactionEntry = transaction;
                        transactionEntry.Type = type;
                        return transactionEntry;
                    },
                    new { id }
                   ).ToList();
                result.IsOk = true;
            }
            catch (Exception e)
            {
                result.ExceptionMessage = e.Message;
            }
            return result;
        }

        public DataWrapper<List<TransactionDto>> GetByAccountId(long accountId)
        {
            var result = new DataWrapper<List<TransactionDto>>();
            try
            {
                string sqlExpression = "Transaction_GetByAccountId @accountId";
                result.Data = _connection.Query<TransactionDto, TransactionTypeDto, TransactionDto>(sqlExpression,
                    (transaction, type) =>
                    {
                        TransactionDto transactionEntry;
                        transactionEntry = transaction;
                        transactionEntry.Type = type;
                        return transactionEntry;
                    },
                    new { accountId },
                    splitOn: "id").ToList();
                result.IsOk = true;
            }
            catch (Exception e)
            {
                result.ExceptionMessage = e.Message;
            }
            return result;
        }

        public DataWrapper<List<TransactionDto>> SearchTransactions(TransactionSearchParameters searchParameters)
        {
            var result = new DataWrapper<List<TransactionDto>>();
            try
            {
                string sqlExpression = "Transaction_Search @accountId, @typeId, @currencyId, @amountBegin, @amountEnd, @fromDate, @tillDate";
                result.Data = _connection.Query<TransactionDto, TransactionTypeDto, CurrencyDto, TransactionDto>(sqlExpression,
                    (transaction, type, currency) =>
                    {
                        TransactionDto transactionEntry;

                        transactionEntry = transaction;
                        transactionEntry.Type = type;
                        transactionEntry.Currency = currency;

                        return transactionEntry;
                    },
                    searchParameters,
                    splitOn: "id").ToList();

                result.IsOk = true;
            }
            catch (Exception e)
            {
                result.ExceptionMessage = e.Message;
            }
            return result;
        }

        public DataWrapper<decimal> GetBalanceByAccountId(long accountId)
        {
            var result = new DataWrapper<decimal>();
            try
            {
                string sqlExpression = "Transaction_GetBalanceByAccountId";
                result.Data = _connection.Query<decimal>(sqlExpression, new { accountId }, commandType: CommandType.StoredProcedure).FirstOrDefault();
                result.IsOk = true;
            }
            catch (Exception e)
            {
                result.ExceptionMessage = e.Message;
            }
            return result;
        }

        public void UpdateCurrencyRates()
        {
            foreach (var c in _currencies.Rates)
            {
                string sqlExpression = "CurrencyRates_Update @code, @rate";
                _connection.Execute(sqlExpression,
                    new
                    {
                        code = c.Code,
                        rate = c.Rate
                    });
            }
        }

        public decimal GetRates(byte currencyId)
        {
            string code = Enum.GetName(typeof(TransactionCurrency), currencyId);
            var currency = _currencies.Rates?.Where(t => t.Code == code).FirstOrDefault();
            if (currency != null)
                return currency.Rate;
            else
                return _connection.Query<decimal>("CurrencyRates_GetById @id", new { id = currencyId }).FirstOrDefault();
        }
    }
}
