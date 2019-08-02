using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Frontier_Test.Models
{
    public class Account
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public decimal AmountDue { get; set; }
        public DateTime? PaymentDueDate { get; set; }
        public AccountStatuses AccountStatusId { get; set; }
    }
    public enum AccountStatuses
    {
        Active,
        Inactive,
        Overdue
    }
}
