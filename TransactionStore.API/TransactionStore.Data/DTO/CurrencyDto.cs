using System;
using System.Collections.Generic;
using System.Text;

namespace TransactionStore.Data.DTO
{
    public class CurrencyDto
    {
        public byte? Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
    }
}
