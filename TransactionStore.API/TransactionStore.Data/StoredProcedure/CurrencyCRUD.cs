﻿using System;
using System.Collections.Generic;
using System.Text;
using TransactionStore.Data.DTO;
using Dapper;
using System.Linq;

namespace TransactionStore.Data.StoredProcedure
{
    public class CurrencyCRUD
    {
        public byte Add(CurrencyDTO currencyDTO)
        {
            var connection = Connection.GetConnection();
            connection.Open();
            string sqlExpression = "Currency_Add @name, @code";
            return connection.Query<byte>(sqlExpression, currencyDTO).FirstOrDefault();
        }
    }
}
