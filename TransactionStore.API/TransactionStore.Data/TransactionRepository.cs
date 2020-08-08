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

            var rates = new ExchangeRates();
         
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
                        ExchangeRates = rates.GetExchangeRates(transactionDto.Currency.Id.Value)

                    }).FirstOrDefault();
                result.IsOk = true;
            }

            catch (Exception e)
            {
                result.ExceptionMessage = e.Message;
            }
            return result;
        }

        public DataWrapper<List<long>> AddTransfer(TransferTransaction transfer)
        {
            var rates = new ExchangeRates();
            decimal exchangeRates1 = rates.GetExchangeRates(transfer.Currency.Id.Value);
            decimal exchangeRates2 = rates.GetExchangeRates(transfer.ReceiverCurrencyId);
            
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

        public decimal GetTotalAmountByAccountId(long accountId)
        {
            decimal balance = 0;
            List<TransactionDto> transactions;
            transactions = GetByAccountId(accountId).Data;
            foreach (var transaction in transactions)
            {
                if (transaction.AccountId != accountId) transaction.Amount *= -1;
                
               
                balance += transaction.Amount;
            }
            return balance;
        }
    }
}
