using System;
using System.Linq;
using System.Collections.Generic;
using System.Data;
using TransactionStore.Data.DTO;
using Dapper;
using TransactionStore.Core.Shared;

namespace TransactionStore.Data
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly IDbConnection _connection;
        public TransactionRepository()
        {
            _connection = Connection.GetConnection();
        }

        public DataWrapper<long> Add(TransactionDto transactionDto) // дженерик с OK, ErrorMessage, Data
        {
            var result = new DataWrapper<long>();
            try
            {
                string sqlExpression = "Transaction_Add @leadId, @typeId, @currencyId, @amount";
                result.Data = _connection.Query<long>(sqlExpression,
                    new
                    {
                        transactionDto.Id,
                        transactionDto.LeadId,
                        TypeId = transactionDto.Type.Id,
                        CurrencyId = transactionDto.Currency.Id,
                        transactionDto.Amount
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

            var result = new DataWrapper<List<long>>();
            try
            {
                string sqlExpression = "Transaction_AddTransfer @leadId, @amount, @currencyId, @leadIdReceiver";
                result.Data = _connection.Query<long>(sqlExpression,
                    new
                    {
                        transfer.Id,
                        transfer.LeadId,
                        transfer.Amount,
                        currencyId = transfer.Currency.Id,
                        transfer.LeadIdReceiver
                    }).ToList();
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
                result.Data = _connection.Query<TransactionDto, TransactionTypeDto, CurrencyDto, TransactionDto>(sqlExpression,
                    (transaction, type, currency) =>
                    {
                        TransactionDto transactionEntry;
                        transactionEntry = transaction;
                        transactionEntry.Type = type;
                        transactionEntry.Currency = currency;
                        transactions.Add(transactionEntry);
                        return transactionEntry;
                    },
                    new { id },
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

        public DataWrapper<List<TransactionDto>> GetByLeadId(long leadId)
        {
            var result = new DataWrapper<List<TransactionDto>>();
            try
            {
                var transactions = new List<TransactionDto>();
                string sqlExpression = "Transaction_GetByLeadId @leadId";
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
                    new { leadId },
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
                string sqlExpression = "Transaction_Search @leadId, @typeId, @currencyId, @amountBegin, @amountEnd, @fromDate, @tillDate";
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

        public decimal GetTotalAmountInCurrency(long leadId, byte currency)
        {
            decimal balance = 0;
            List<TransactionDto> transactions;
            transactions = GetByLeadId(leadId).Data;
            foreach (var transaction in transactions)
            {
                if (transaction.LeadId != leadId) transaction.Amount *= -1;
                if (currency == (byte)TransactionCurrency.RUR)
                {
                    if (transaction.Currency.Id == (byte)TransactionCurrency.USD) transaction.Amount *= 71;
                    if (transaction.Currency.Id == (byte)TransactionCurrency.EUR) transaction.Amount *= 80;
                }
                if (currency == (byte)TransactionCurrency.USD)
                {
                    if (transaction.Currency.Id == (byte)TransactionCurrency.RUR) transaction.Amount /= 71;
                    if (transaction.Currency.Id == (byte)TransactionCurrency.EUR) transaction.Amount *= (decimal)0.89;
                }
                if (currency == (byte)TransactionCurrency.EUR)
                {
                    if (transaction.Currency.Id == 1) transaction.Amount /= 80;
                    if (transaction.Currency.Id == (byte)TransactionCurrency.USD) transaction.Amount *= (decimal)1.13;
                }
                balance += transaction.Amount;
            }
            return balance;
        }
    }
}
