using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Nuttawat_7324_L002.Models;
using Nuttawat_7324_L002.Services;
using Nuttawat_7324_L002.ViewModels;
using Xunit;

namespace CustomShop.Tests;

public sealed class QuoteEstimateRulesTests
{
    [Theory]
    [InlineData(null, true)]
    [InlineData(0, true)]
    [InlineData(12, true)]
    [InlineData(1000, true)]
    [InlineData(1001, false)]
    public void EstimateNotesAreOptionalAndLimitedToOneThousandCharacters(int? length, bool expectedValid)
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddControllers();
        using var provider = services.BuildServiceProvider();
        using var scope = provider.CreateScope();
        var context = new ActionContext(
            new DefaultHttpContext { RequestServices = scope.ServiceProvider },
            new RouteData(),
            new ActionDescriptor());
        var form = new QuoteEstimateForm
        {
            ProjectCode = "PRJ-TEST",
            Notes = length is int count ? new string('ก', count) : null
        };
        var validator = scope.ServiceProvider.GetRequiredService<IObjectModelValidator>();

        validator.Validate(context, validationState: null, prefix: "Form", model: form);

        Assert.Equal(expectedValid, context.ModelState.IsValid);
        if (!expectedValid)
            Assert.Single(context.ModelState["Form.Notes"]!.Errors);
    }

    [Fact]
    public void TotalUsesQuantitiesAndFeesAndDeductsDiscount()
    {
        (int, decimal)[] lines = [(2, 120.25m), (1, 15m)];

        var total = QuoteEstimateRules.Total(lines, 500m, 50m, 100m);

        Assert.Equal(705.50m, total);
    }

    [Fact]
    public void SupportsLegacyQuoteWithoutPartsAndFullDiscount()
    {
        (int, decimal)[] lines = [];

        var total = QuoteEstimateRules.Total(lines, 0m, 7_500m, 7_500m);

        Assert.Equal(0m, total);
    }

    [Theory]
    [InlineData(0, "5", "0")]
    [InlineData(1, "-1", "0")]
    [InlineData(1, "1.001", "0")]
    [InlineData(1, "5", "6")]
    [InlineData(2, "6000000", "0")]
    public void TotalRejectsInvalidAmounts(int quantity, string price, string discount)
    {
        (int, decimal)[] lines = [(quantity, decimal.Parse(price, System.Globalization.CultureInfo.InvariantCulture))];

        Action action = () => QuoteEstimateRules.Total(lines, 0m, 0m, decimal.Parse(discount, System.Globalization.CultureInfo.InvariantCulture));

        Assert.Throws<ShopException>(action);
    }

    [Fact]
    public void OnlyCurrentUnreviewedSubmissionCanBeReviewed()
    {
        var estimate = new QuoteEstimate { Revision = 2, EditVersion = 3, SubmittedAt = DateTime.UnixEpoch };

        Assert.True(QuoteEstimateRules.MatchesVersion(estimate, 2, 3));
        Assert.False(QuoteEstimateRules.MatchesVersion(estimate, 2, 2));
        Assert.True(QuoteEstimateRules.MatchesVersion(null, 0, 0));
        Assert.True(QuoteEstimateRules.IsAwaitingReview(estimate, 2));
        Assert.False(QuoteEstimateRules.IsAwaitingReview(estimate, 1));

        estimate.ReviewedAt = DateTime.UnixEpoch;

        Assert.False(QuoteEstimateRules.IsAwaitingReview(estimate, 2));
    }

    [Theory]
    [InlineData("engine", "engine")]
    [InlineData("engine", "unknown")]
    [InlineData("engine", "")]
    public void PricingRejectsDuplicateForeignOrMissingParts(string first, string second)
    {
        ProjectPart[] parts = [new() { SlotCode = "engine" }, new() { SlotCode = "brakes" }];
        var inputs = new List<QuoteEstimatePriceForm> { new() { SlotCode = first, UnitPrice = 400m } };
        if (second.Length > 0) inputs.Add(new() { SlotCode = second, UnitPrice = 300m });

        Action save = () => QuoteEstimateRules.PricesForParts(parts, inputs);

        Assert.Throws<ShopException>(save);
    }

    [Fact]
    public void PricingBindsAmountsToTheRequestSlotsRegardlessOfInputOrder()
    {
        ProjectPart[] parts = [new() { SlotCode = "engine" }, new() { SlotCode = "brakes" }];
        QuoteEstimatePriceForm[] inputs = [new() { SlotCode = "brakes", UnitPrice = 100 }, new() { SlotCode = "engine", UnitPrice = 300 }];

        var prices = QuoteEstimateRules.PricesForParts(parts, inputs);

        Assert.Equal(300m, prices["engine"]);
        Assert.Equal(100m, prices["brakes"]);
    }
}
