using System;
using System.Text;
using System.Data;
using System.Linq;
using System.Collections.Generic;
using TransactionStore.Data.DTO;
using Dapper;

namespace TransactionStore.Data
{
    public class TransactionRepository
    {
        public long Add(TransactionDto transactionDTO)
        {
            var connection = Connection.GetConnection();
            connection.Open();
            string sqlExpression = "Transaction_Add @leadId, @typeId, @currencyId, @amount, @timestamp";
            return connection.Query<long>(sqlExpression, transactionDTO).FirstOrDefault();
        }

        public List<TransactionDto> GetByLeadId(long leadId)
        {
            var connection = Connection.GetConnection();
            connection.Open();
            string sqlExpression = "Transaction_GetByLeadId @leadId";
            return connection.Query<TransactionDto>(sqlExpression, new { leadId }).ToList();
        }

        public List<long> AddTransfer(TransferTransactionDto transfer)
        {
            var connection = Connection.GetConnection();
            connection.Open();
            string sqlExpression = "Transaction_AddTransfer  @leadId, @typeId, @currencyId, @amount, @destinationLeadId";
            return connection.Query<long>(sqlExpression, transfer).ToList();
        }

        public List<TransactionDto> GetById(long id)
        {
            var connection = Connection.GetConnection();
            connection.Open();
            string sqlExpression = "Transaction_GetById @id";
            return connection.Query<TransactionDto>(sqlExpression, new { id }).ToList();
        }

        public decimal GetTotalAmount(long leadId)
        {
            var connection = Connection.GetConnection();
            connection.Open();
            string sqlExpression = "TotalAmount @leadId";
            return connection.Query<decimal>(sqlExpression, new { leadId }).FirstOrDefault();
        }
    }
}
