using System;
using System.Collections.Generic;
using System.Text;

namespace TransactionStore.Data.DTO
{
    public class CurrencyDTO
    {
        public CurrencyDTO(byte id, string name, string code)
        {
            Id = id;
            Name = name;
            Code = code;
        }


        public byte Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
    }
}
