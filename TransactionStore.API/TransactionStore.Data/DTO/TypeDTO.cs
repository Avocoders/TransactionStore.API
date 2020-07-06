using System;
using System.Collections.Generic;
using System.Text;

namespace TransactionStore.Data.DTO
{
    public class TypeDTO
    {
        public byte Id { get; set; }
        public string Name { get; set; }
        
        public TypeDTO(byte id, string name)
        {
            Id = id;
            Name = name;            
        }
    }
}
