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
        public TransactionRepository(IOptions<StorageOptions> options)
        {
            _connection = new SqlConnection(options.Value.DBConnectionString);
        }

        public DataWrapper<long> Add(TransactionDto transactionDto) 
        {            
            string currency = Enum.GetName(typeof(TransactionCurrency), transactionDto.Currency.Id.Value);
            var result = new DataWrapper<long>();
            try
            {
                string sqlExpression = "Transaction_Add @accountId, @typeId, @currencyId, @amount, @exchangeRates";
                result.Data = _connection.Query<long>(sqlExpression,
                    new
                    {
                        transactionDto.Id,
                        transactionDto.AccountId,
                        TypeId = transactionDto.Type.Id,
                        CurrencyId = transactionDto.Currency.Id,
                        transactionDto.Amount,
                        ExchangeRates = Currencies.Rates[currency]

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
            string currency1 = Enum.GetName(typeof(TransactionCurrency), transfer.Currency.Id.Value);
            string currency2 = Enum.GetName(typeof(TransactionCurrency), transfer.ReceiverCurrencyId);
            decimal exchangeRates1 = Currencies.Rates[currency1];
            decimal exchangeRates2 = Currencies.Rates[currency2];

            var result = new DataWrapper<List<long>>();
            try
            {
                string sqlExpression = "Transaction_AddTransfer ";
                result.Data = _connection.Query<long>(sqlExpression,
                    new
                    {
                        accountId = transfer.AccountId,
                        typeId = transfer.Type.Id,
                        currencyId = transfer.Currency.Id,
                        amount1 = transfer.Amount,
                        Amount2 = transfer.Amount / exchangeRates1 * exchangeRates2,
                        accountIdReceiver = transfer.AccountIdReceiver,
                        receiverCurrencyId = transfer.ReceiverCurrencyId,
                        exchangeRates1,
                        exchangeRates2 

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
                var transactions = new List<TransactionDto>();
                string sqlExpression = "Transaction_GetById @id";
                result.Data = _connection.Query<TransactionDto, TransactionTypeDto, TransactionDto>(sqlExpression,
                    (transaction, type) =>
                    {
                        TransactionDto transactionEntry;
                        transactionEntry = transaction;
                        transactionEntry.Type = type;
                        transactions.Add(transactionEntry);
                        return transactionEntry;
                    },
                    new { id }
                   ).ToList();
                result.Data = transactions;
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
                var transactions = new List<TransactionDto>();
                string sqlExpression = "Transaction_GetByAccountId @accountId";
                var data = _connection.Query<TransactionDto, TransactionTypeDto, TransactionDto>(sqlExpression,
                    (transaction, type) =>
                    {
                        TransactionDto transactionEntry;
                        transactionEntry = transaction;
                        transactionEntry.Type = type;
                        transactions.Add(transactionEntry);
                        return transactionEntry;
                    },
                    new { accountId },
                    splitOn: "id").ToList();
                result.Data = transactions;
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
                var transactions = new List<TransactionDto>();
                string sqlExpression = "Transaction_Search @accountId, @typeId, @currencyId, @amountBegin, @amountEnd, @fromDate, @tillDate";
                var data = _connection.Query<TransactionDto, TransactionTypeDto, CurrencyDto, TransactionDto>(sqlExpression,
                    (transaction, type, currency) =>
                    {
                        TransactionDto transactionEntry;

                        transactionEntry = transaction;
                        transactionEntry.Type = type;
                        transactionEntry.Currency = currency;
                        transactions.Add(transactionEntry);

                        return transactionEntry;
                    },
                    searchParameters,
                    splitOn: "id").ToList();

                result.Data =transactions;
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
                var balance = _connection.Query<decimal>(sqlExpression, new { accountId }, commandType: CommandType.StoredProcedure).FirstOrDefault();
                result.Data = balance;
                result.IsOk = true;
            }
            catch (Exception e)
            {
                result.ExceptionMessage = e.Message;
            }
            return result;
        }
    }
}
