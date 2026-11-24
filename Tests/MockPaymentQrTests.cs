using System.ComponentModel.DataAnnotations;
using Nuttawat_7324_L002.Services;
using Nuttawat_7324_L002.ViewModels;
using Xunit;

namespace CustomShop.Tests;

public class MockPaymentQrTests
{
    [Theory]
    [InlineData("abcd", "DEMO-ABCD")]
    [InlineData("abcdef0123456789", "DEMO-ABCDEF012345")]
    public void ReferenceIsStableAndClearlyMarkedAsDemo(string revision, string expected)
    {
        var reference = MockPaymentQr.Reference(revision);

        Assert.Equal(expected, reference);
    }

    [Fact]
    public void QrEncodesDemoPayloadAndProducesPng()
    {
        const string reference = "DEMO-ABC";
        const decimal amount = 129.5m;

        var payload = MockPaymentQr.Payload(reference, amount);
        var image = MockPaymentQr.ImageDataUrl(reference, amount);

        Assert.Equal("CustomShop DEMO ONLY / DEMO-ABC / THB 129.50", payload);
        Assert.StartsWith("data:image/png;base64,iVBORw0KGgo", image);
    }

    [Theory]
    [InlineData("success", true)]
    [InlineData("failure", true)]
    [InlineData("paid", false)]
    public void CheckoutFormAcceptsOnlyKnownSimulationOutcomes(string result, bool expected)
    {
        var form = new MockCheckoutForm { Revision = "cart123", Fulfillment = "pickup", ExpectedTotal = 0, Result = result };

        var valid = Validator.TryValidateObject(form, new ValidationContext(form), null, true);

        Assert.Equal(expected, valid);
    }
}
