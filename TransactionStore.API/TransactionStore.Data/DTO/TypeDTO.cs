using System;
using System.Collections.Generic;
using System.Text;

namespace TransactionStore.Data.DTO
{
    public class TypeDTO
    {
        public TypeDTO(byte id, string name)
        {
            Id = id;
            Name = name;
        }


        public byte? Id { get; set; }
        public string Name { get; set; }
    }
}
