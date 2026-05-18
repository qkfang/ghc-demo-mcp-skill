using App.Functions.Mappings;
using App.Functions.Options;

namespace App.Functions.Tests;

public class OrderMappingsTests
{
    private static readonly PricingOptions Pricing = new()
    {
        TierOneMaxTickets = 5,
        TierTwoMaxTickets = 10,
        TierOnePrice = 100,
        TierTwoPrice = 90,
        TierThreePrice = 80
    };

    [Theory]
    [InlineData(5, 500)]
    [InlineData(10, 900)]
    [InlineData(12, 960)]
    public void CalculatePrice_UsesExpectedTieredPricing(int noTickets, int expectedPrice)
    {
        var price = OrderMappings.CalculatePrice(noTickets, Pricing);
        Assert.Equal(expectedPrice, price);
    }
}
