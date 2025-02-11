using FakeItEasy;
using IPBot.API.Domain.Data;
using IPBot.API.Domain.Repositories;
using IPBot.API.Hubs;
using IPBot.API.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using MockQueryable;

namespace IPBot.API.Tests;

public class IPServiceTests
{
    [Fact]
    public async Task GetCurrentServerDomain_WhenCalled_ReturnsCorrectURL()
    {
        // Arrange
        var fakeDbContext = A.Fake<IIPBotDataContext>();
        var fakeHubContext = A.Fake<IHubContext<IPHub>>();
        var fakeHttpClientFactory = A.Fake<IHttpClientFactory>();

        var fakeDbSet = A.Fake<DbSet<Domain.Entities.Domain>>(d => d.Implements<IQueryable<Domain.Entities.Domain>>());

        var fakeIQueryable = new List<Domain.Entities.Domain>
        {
            new()
            {
                Id = 1,
                Description = "Server Domain",
                URL = "test.url"
            }
        }
        .AsQueryable()
        .BuildMock();

        A.CallTo(() => ((IQueryable<Domain.Entities.Domain>)fakeDbSet).GetEnumerator())
            .Returns(fakeIQueryable.GetEnumerator());
        A.CallTo(() => ((IQueryable<Domain.Entities.Domain>)fakeDbSet).Provider)
            .Returns(fakeIQueryable.Provider);
        A.CallTo(() => ((IQueryable<Domain.Entities.Domain>)fakeDbSet).Expression)
            .Returns(fakeIQueryable.Expression);
        A.CallTo(() => ((IQueryable<Domain.Entities.Domain>)fakeDbSet).ElementType)
          .Returns(fakeIQueryable.ElementType);

        A.CallTo(() => fakeDbContext.Domains).Returns(fakeDbSet);
        A.CallTo(() => fakeDbContext.Set<Domain.Entities.Domain>()).Returns(fakeDbSet);

        var domainRepository = new DomainRepository(fakeDbContext);
        var ipService = new IPService(domainRepository, fakeHubContext, fakeHttpClientFactory);

        // Act
        var result = await ipService.GetCurrentServerDomainAsync();

        // Assert
        Assert.Equal("test.url", result);
    }
}
