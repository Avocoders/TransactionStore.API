using System;
using System.Collections.Generic;
using System.Text;

namespace TransactionStore.Core
{
    public interface IStorageOptions
    {
       public string DBConnectionString { get; set; }
    }
}
