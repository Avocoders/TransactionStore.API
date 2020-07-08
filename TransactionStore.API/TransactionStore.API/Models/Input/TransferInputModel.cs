using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TransactionStore.API.Models.Input
{
    public class TransferInputModel :TransactionInputModel
    {
        public long DestinationLeadId { get; set; }
    }
}