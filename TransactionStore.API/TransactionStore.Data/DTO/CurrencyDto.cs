using System;

namespace TransactionStore.Data.DTO
{
    public class CurrencyDto
    {
        public CurrencyDto(byte id, string name, string code)
        {
            Id = id;
            Name = name;
            Code = code;
        }


        public byte? Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
    }
}
