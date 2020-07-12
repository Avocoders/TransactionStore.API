using System;
using System.Collections.Generic;
using System.Text;

namespace TransactionStore.Data.DTO
{
    public class RangeDateDto: TransactionDto
    {
        public DateTime FromDate { get; set; }
        public DateTime TillDate { get; set; }
    }
}
