using System;
using System.Collections.Generic;
using System.Text;

namespace TransactionStore.Core
{
    public class StorageOptions : IStorageOptions
    {
        public string DBConnectionString { get; set; }
    }
}
