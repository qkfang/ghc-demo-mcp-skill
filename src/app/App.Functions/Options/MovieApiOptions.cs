namespace App.Functions.Options;

public sealed class MovieApiOptions
{
    public PricingOptions Pricing { get; set; } = new();

    public string? MoviesJson { get; set; }
}

public sealed class PricingOptions
{
    public int TierOneMaxTickets { get; set; } = 5;

    public int TierTwoMaxTickets { get; set; } = 10;

    public int TierOnePrice { get; set; } = 100;

    public int TierTwoPrice { get; set; } = 90;

    public int TierThreePrice { get; set; } = 80;
}
