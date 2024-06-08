namespace Kolokwium_CodeFirst.DTOs;

public class PaymentDto
{
    public int IdClient { get; set; }
        public int IdSubscription { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
    }