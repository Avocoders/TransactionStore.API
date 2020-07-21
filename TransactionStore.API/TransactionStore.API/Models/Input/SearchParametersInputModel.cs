using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TransactionStore.API.Models.Input
{
    public class SearchParametersInputModel
    {
        public long? LeadId { get; set; }
        public byte? Type { get; set; }
        public byte? Currency { get; set; }
        public decimal? AmountBegin { get; set; }
        public decimal? AmountEnd { get; set; }
        public string FromDate { get; set; }
        public string TillDate { get; set; }
    }
}
