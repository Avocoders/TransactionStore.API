using System;
using System.Linq;
using System.Collections.Generic;
using System.Data;
using TransactionStore.Data.DTO;
using Dapper;

namespace TransactionStore.Data
{
    public class TransactionRepository
    {
        private readonly IDbConnection _connection;
        public TransactionRepository()
        {
            _connection = Connection.GetConnection();
        }
        
        public long Add(TransactionDto transactionDto) // дженерик с OK, ErrorMessage, Data
        {
            try
            {
                string sqlExpression = "Transaction_Add @leadId, @typeId, @currencyId, @amount";
                return _connection.Query<long>(sqlExpression, transactionDto).FirstOrDefault();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public List<TransactionDto> GetByLeadId(long leadId)
        {
            string sqlExpression = "Transaction_GetByLeadId @leadId";
            return _connection.Query<TransactionDto>(sqlExpression, new { leadId }).ToList();
        }

        public List<long> AddTransfer(TransferTransactionDto transfer)
        {
            string sqlExpression = "Transaction_AddTransfer  @leadId, @typeId, @currencyId, @amount, @destinationLeadId";
            return _connection.Query<long>(sqlExpression, transfer).ToList();
        }

        public TransactionDto GetById(long id)
        {
            string sqlExpression = "Transaction_GetById @id";
            return _connection.Query<TransactionDto>(sqlExpression, new { id }).FirstOrDefault();
        }

        public decimal GetTotalAmount(long leadId)
        {
            string sqlExpression = "TotalAmount @leadId";
            return _connection.Query<decimal>(sqlExpression, new { leadId }).FirstOrDefault();
        }
    }
}
