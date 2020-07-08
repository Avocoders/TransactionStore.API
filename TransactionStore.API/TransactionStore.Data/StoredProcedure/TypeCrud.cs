using System;
using System.Collections.Generic;
using System.Text;
using TransactionStore.Data.DTO;
using Dapper;
using System.Linq;

namespace TransactionStore.Data.StoredProcedure
{
    public class TypeCrud
    {
        public byte Add(TypeDto typeDTO)
        {
            var connection = Connection.GetConnection();
            connection.Open();
            string sqlExpression = "Type_Add @name";
            return connection.Query<byte>(sqlExpression, typeDTO).FirstOrDefault();
        }
    }
}
