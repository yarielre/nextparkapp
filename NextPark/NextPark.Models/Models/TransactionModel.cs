
using System;
using NextPark.Enums.Enums;

namespace NextPark.Models.Models
{
   public class TransactionModel:BaseModel
    {
        public string TransactionId { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime CompletationDate { get; set; }
        public TransactionStatus Status { get; set; }
        public TransactionType Type { get; set; }
        public double CashMoved { get; set; }
        public string  User { get; set; }
        public int UserId { get; set; }
        public string PurchaseId { get; set; }
        public string PurchaseToken { get; set; }
        public string PurchaseState { get; set; }

        //Only for views purpose
        public string TransactionStatus { get; set; }
        public string TransactionType { get; set; }
        public string CreationTime { get; set; }
        public string CompleteTime { get; set; }
    }
}
