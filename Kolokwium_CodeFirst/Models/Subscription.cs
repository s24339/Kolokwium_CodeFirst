namespace Kolokwium_CodeFirst.Models;

public class Subscription
{
    public int IdSubscription { get; set; }
    public string Name { get; set; }
    public int RenewalPeriod { get; set; }  // 1, 3, 6 months

    public ICollection<Sale> Sales { get; set; }
}