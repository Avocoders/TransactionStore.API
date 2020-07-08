using System;
using System.Collections.Generic;
using System.Text;
using TransactionStore.Data.DTO;
using Dapper;
using Microsoft.AspNetCore.Http;
using System.Linq;

namespace TransactionStore.Data.StoredProcedure
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
        
        public List<TransactionDto> GetById(long id)
        {
            var connection = Connection.GetConnection();
            connection.Open();
            string sqlExpression = "Transaction_GetById @id";
            return connection.Query<TransactionDto>(sqlExpression, new { id }).ToList();
        }
    }
}
