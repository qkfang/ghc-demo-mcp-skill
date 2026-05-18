using App.Functions.Models;
using App.Functions.Options;

namespace App.Functions.Mappings;

public static class OrderMappings
{
    public static int CalculatePrice(int noTickets, PricingOptions pricing)
    {
        var perTicket = noTickets <= pricing.TierOneMaxTickets
            ? pricing.TierOnePrice
            : noTickets <= pricing.TierTwoMaxTickets
                ? pricing.TierTwoPrice
                : pricing.TierThreePrice;

        return noTickets * perTicket;
    }

    public static OrderDetail CreateOrder(int orderId, int movieId, int noTickets, PricingOptions pricing)
    {
        return new OrderDetail
        {
            OrderId = orderId,
            MovieId = movieId,
            NoTickets = noTickets,
            Price = CalculatePrice(noTickets, pricing)
        };
    }
}
