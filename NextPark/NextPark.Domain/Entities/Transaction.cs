using NextPark.Domain.Core;
using System;
using NextPark.Enums.Enums;

namespace NextPark.Domain.Entities
{
   public class Transaction:BaseEntity
    {
        public Guid TransactionId { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime CompletationDate { get; set; }
        public TransactionStatus Status { get; set; }
        public TransactionType Type { get; set; }
        public double CashMoved { get; set; }
        public virtual  ApplicationUser User { get; set; }
        public int UserId { get; set; }
    }
}
