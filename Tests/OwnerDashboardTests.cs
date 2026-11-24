using System.ComponentModel.DataAnnotations;
using Nuttawat_7324_L002.Models;
using Nuttawat_7324_L002.ViewModels;
using Xunit;

namespace CustomShop.Tests;

public class OwnerDashboardTests
{
    [Theory]
    [InlineData("PRJ-123", "PRJ-123")]
    [InlineData("PRJ-123456789012", "PRJ-123456789012")]
    [InlineData("PRJ-1234567890123456789", "PRJ-12345678…")]
    public void DisplayCodeFormatsReferenceWithoutChangingProjectIdentifier(string code, string expected)
    {
        var row = new OwnerProjectRowViewModel { Project = new Project { ProjectCode = code } };

        var displayed = row.DisplayCode;

        Assert.Equal(expected, displayed);
        Assert.Equal(code, row.Project.ProjectCode);
    }

    [Theory]
    [InlineData("PRJ-1", "ช่างหนึ่ง", 0, true)]
    [InlineData("PRJ-1", "ช่างหนึ่ง", 10_000_000, true)]
    [InlineData("PRJ-1", "ช่างหนึ่ง", -1, false)]
    [InlineData("PRJ-1", "ช่างหนึ่ง", 10_000_001, false)]
    [InlineData("PRJ-1", "", 100, false)]
    [InlineData("", "ช่างหนึ่ง", 100, false)]
    public void AssignmentValidatesRequiredFieldsAndBudget(string code, string mechanic, int budget, bool expected)
    {
        var form = new OwnerAssignmentViewModel { ProjectCode = code, Mechanic = mechanic, Budget = budget };

        var valid = Validator.TryValidateObject(form, new ValidationContext(form), new List<ValidationResult>(), true);

        Assert.Equal(expected, valid);
    }
}
