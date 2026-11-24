using Nuttawat_7324_L002.Models;
using Nuttawat_7324_L002.Services;
using Xunit;

namespace CustomShop.Tests;

public class OwnerDepartmentTests
{
    [Theory]
    [InlineData("Guest")]
    [InlineData("guest")]
    [InlineData("GUEST")]
    public void SummariesExcludeGuestAndKeepOtherRolesAndTheirActiveHeadCounts(string guestName)
    {
        var roles = new[]
        {
            new Role { Name = guestName, Staff = [new Staff { Status = "active" }] },
            new Role { Name = AppRoles.Mechanic, Staff = [new Staff { Status = "active" }, new Staff { Status = "suspended" }] },
            new Role { Name = AppRoles.Owner }
        };

        var departments = OwnerDepartmentQuery.Summaries(roles.AsQueryable()).ToArray();

        Assert.Collection(departments,
            mechanic =>
            {
                Assert.Equal(AppRoles.Mechanic, mechanic.DepartmentName);
                Assert.Equal(1, mechanic.HeadCount);
                Assert.Equal("มีพนักงานประจำ", mechanic.Status);
            },
            owner =>
            {
                Assert.Equal(AppRoles.Owner, owner.DepartmentName);
                Assert.Equal(0, owner.HeadCount);
                Assert.Equal("ไม่มีพนักงานประจำ", owner.Status);
            });
    }
}
