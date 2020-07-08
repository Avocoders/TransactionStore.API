using System;

namespace TransactionStore.Data.DTO
{
    public class TypeDto
    {
        public TypeDto(byte id, string name)
        {
            Id = id;
            Name = name;
        }


        public byte? Id { get; set; }
        public string Name { get; set; }
    }
}
