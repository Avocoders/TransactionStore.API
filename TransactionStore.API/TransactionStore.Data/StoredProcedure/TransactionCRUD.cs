using System;
using System.Collections.Generic;
using System.Text;
using TransactionStore.Data.DTO;
using Dapper;
using Microsoft.AspNetCore.Http;
using System.Linq;

namespace TransactionStore.Data.StoredProcedure
{
    public class TransactionCRUD
    {
        public long Add(TransactionDTO transactionDTO)
        {
            var connection = Connection.GetConnection();
            connection.Open();
            string sqlExpression = "Transaction_Add @leadId, @typeId, @currencyId, @amount, @timestamp";
            return connection.Query<long>(sqlExpression, transactionDTO).FirstOrDefault();
        }

        public List<TransactionDTO> GetByLeadId(long leadId)
        {
            var connection = Connection.GetConnection();
            connection.Open();
            string sqlExpression = "Transaction_GetByLeadId @leadId";
            return connection.Query<TransactionDTO>(sqlExpression, new { leadId }).ToList();
        }
    }
}
