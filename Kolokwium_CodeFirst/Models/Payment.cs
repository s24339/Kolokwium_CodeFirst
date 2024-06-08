namespace Kolokwium_CodeFirst.Models;

public class Payment
{
    public int IdPayment { get; set; }
    public int IdSale { get; set; }
    public decimal Amount { get; set; }
    public DateTime PaymentDate { get; set; }

    public Sale Sale { get; set; }
}