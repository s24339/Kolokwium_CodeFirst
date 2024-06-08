using Kolokwium_CodeFirst.Context;
using Kolokwium_CodeFirst.DTOs;
using Kolokwium_CodeFirst.Models;

namespace Kolokwium_CodeFirst.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class ClientController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ClientController(ApplicationDbContext context)
    {
        _context = context;
    }
    
    [HttpGet("{idClient}")]
    public async Task<IActionResult> GetClientWithSubscriptions(int idClient)
    {
        var client = await _context.Clients
            .Include(c => c.Sales)
                .ThenInclude(s => s.Subscription)
            .Include(c => c.Sales)
                .ThenInclude(s => s.Payments)
            .FirstOrDefaultAsync(c => c.IdClient == idClient);

        if (client == null)
        {
            return NotFound(new { error = "Client not found" });
        }

        var result = new
        {
            firstName = client.FirstName,
            lastName = client.LastName,
            email = client.Email,
            phone = client.Phone,
            subscriptions = client.Sales.GroupBy(s => s.Subscription)
                                        .Select(g => new
                                        {
                                            IdSubscription = g.Key.IdSubscription,
                                            Name = g.Key.Name,
                                            TotalPaidAmount = g.SelectMany(s => s.Payments).Sum(p => p.Amount)
                                        })
                                        .ToList()
        };

        return Ok(result);
    }
    
    [HttpPost("payment")]
    public async Task<IActionResult> AddPayment([FromBody] PaymentDto paymentDto)
    {
        var client = await _context.Clients.FindAsync(paymentDto.IdClient);
        if (client == null)
        {
            return NotFound(new { error = "Client not found" });
        }

        var subscription = await _context.Subscriptions.FindAsync(paymentDto.IdSubscription);
        if (subscription == null)
        {
            return NotFound(new { error = "Subscription not found" });
        }

        var sale = await _context.Sales
            .Where(s => s.IdClient == paymentDto.IdClient && s.IdSubscription == paymentDto.IdSubscription)
            .OrderByDescending(s => s.CreatedAt)
            .FirstOrDefaultAsync();

        if (sale == null)
        {
            return BadRequest(new { error = "No sales found for the client and subscription" });
        }

        var nextPaymentStartDate = sale.CreatedAt.AddMonths(subscription.RenewalPeriod);
        if (paymentDto.PaymentDate < sale.CreatedAt || paymentDto.PaymentDate > nextPaymentStartDate)
        {
            return BadRequest(new { error = "Payment date is outside the subscription period" });
        }

        var payment = new Payment
        {
            IdSale = sale.IdSale,
            Amount = paymentDto.Amount,
            PaymentDate = paymentDto.PaymentDate
        };

        _context.Payments.Add(payment);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Payment added successfully" });
    }
}
