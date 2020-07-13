using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TransactionStore.API.Models.Input
{
    public class RangeDateInputModel
    {
        public string FromDate { get; set; }
        public string TillDate { get; set; }
    }
}
