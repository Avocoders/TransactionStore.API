using System;
using System.Linq;
using System.Collections.Generic;
using System.Data;
using TransactionStore.Data.DTO;
using Dapper;
using TransactionStore.Core.Shared;

namespace TransactionStore.Data
{
    public class TransactionRepository
    {
        private readonly IDbConnection _connection;
        public TransactionRepository()
        {
            _connection = Connection.GetConnection();
        }
        
        public DataWrapper<long> Add(TransactionDto transactionDto) // дженерик с OK, ErrorMessage, Data
        {
            try
            {
                string sqlExpression = "Transaction_Add @leadId, @typeId, @currencyId, @amount";
                return new DataWrapper<long>()
                {
                    Data = _connection.Query<long>(sqlExpression, transactionDto).FirstOrDefault(),
                    IsOk = true
                };
            }

            catch (Exception e)
            {
                return new DataWrapper<long>()
                {
                    ExceptionMessage = e.Message
                };
            }
        }

        public DataWrapper<List<TransferTransaction>> GetByLeadId(long leadId)
        {
            try
            {
                string sqlExpression = "Transaction_GetByLeadId @leadId";
                return new DataWrapper<List<TransferTransaction>>()
                {
                    Data = _connection.Query<TransferTransaction>(sqlExpression, new { leadId }).ToList(),
                    IsOk = true
                };
            }

            catch (Exception e)
            {
                return new DataWrapper<List<TransferTransaction>>()
                {
                    ExceptionMessage = e.Message
                };
            }
        }

        public DataWrapper<List<long>> AddTransfer(TransferTransaction transfer)
        {
            try
            {
                string sqlExpression = "Transaction_AddTransfer @leadId, @typeId, @currencyId, @amount, @destinationLeadId";
                return new DataWrapper<List<long>>()
                {
                    Data = _connection.Query<long>(sqlExpression, transfer).ToList(),
                    IsOk = true
                };
            }

            catch (Exception e)
            {
                return new DataWrapper<List<long>>()
                {
                    ExceptionMessage = e.Message
                };
            }
        }

        public DataWrapper<TransferTransaction> GetById(long id)
        {
            try
            {
                string sqlExpression = "Transaction_GetById @id";
                return new DataWrapper<TransferTransaction>()
                {
                    Data = _connection.Query<TransferTransaction>(sqlExpression, new { id }).FirstOrDefault(),
                    IsOk = true
                };
            }

            catch (Exception e)
            {
                return new DataWrapper<TransferTransaction>()
                {
                    ExceptionMessage = e.Message
                };
            }
        }       
        
        public DataWrapper<List<TransactionDto>> SearchTransactions(TransactionSearchParameters searchParameters)
        {
            try
            {
                string sqlExpression = "Transaction_Search @leadId, @type, @currency, @amount, @fromDate, @tillDate";
                var data = _connection.Query<TransactionDto>(sqlExpression, searchParameters);
                var nonTransferTransactions = data.Where(t => t.Type.Id != (byte)TransactionType.Transfer).ToList();
                var transferTransactions = ConvertTransactionDtosToTransferTransactions(data.Where(t => t.Type.Id == (byte)TransactionType.Transfer).ToList());

                return ;
            }
            catch(Exception e)
            {
                return new DataWrapper<List<TransactionDto>>()
                {
                    ExceptionMessage = e.Message
                };
            }
        }

        private DataWrapper<List<TransferTransaction>> ConvertTransactionDtosToTransferTransactions(List<TransactionDto> transactions)
        {
            List<TransferTransaction> res = new List<TransferTransaction>();
            var transfers = transactions.Where(t => t.Amount < 0).ToList();
            var transfersRecipient = transactions.Where(t => t.Amount > 0).ToList();
            
        }

        public decimal GetTotalAmountInCurrency(long leadId, byte currency)
        {
            decimal balance=0;
            List<TransferTransaction> transactions = new List<TransferTransaction>();
            transactions = GetByLeadId(leadId).Data;
            foreach(var transaction in transactions)
            {
                if (currency == 1)
                {
                    if (transaction.Currency.Id == 2) transaction.Amount *= 71;
                    if (transaction.Currency.Id == 3) transaction.Amount *= 80;
                }
                
                if(currency == 2)
                {
                    if (transaction.Currency.Id == 1) transaction.Amount /= 71;
                    if (transaction.Currency.Id == 3) transaction.Amount *= (decimal)0.89;
                }
                if(currency == 3)
                {
                    if (transaction.Currency.Id == 1) transaction.Amount /= 80;
                    if (transaction.Currency.Id == 2) transaction.Amount *= (decimal)1.13;
                }

                balance += transaction.Amount;
            }
            return balance;
        }
    }
}
