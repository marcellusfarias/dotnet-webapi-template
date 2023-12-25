using MyBuyingList.Application.Features.BuyingLists.DTOs;
using MyBuyingList.Web.Tests.IntegrationTests.Common;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace MyBuyingList.Web.Tests.IntegrationTests;

public class BuyingListControllerTests : BaseIntegrationTest
{
    private readonly HttpClient _client;
    public BuyingListControllerTests(ResourceFactory factory)
        : base(factory)
    {
        _client = factory.HttpClient;
    }

    private async Task<string> GetJwtToken()
    {
        var response = await _client.GetAsync("api/auth?username=admin&password=123");
        var token = await response.Content.ReadAsStringAsync();
        return token;
    }

    [Fact]
    public async void GetBuyingListByAsync_ShouldReturnBuyingList_WhenIdIsOk()
    {
        // Arrange
        var expectedBuyingList = new GetBuyingListDto()
        {
            Name = "Default",
            GroupId = 1,
            CreatedBy = 1,
            CreatedAt = DateTime.Now,
        };

        // Act
        var response = await _client.GetAsync("api/buyinglist?buyingListId=1");

        // Assert
        var buyingList = await response.Content.ReadFromJsonAsync<GetBuyingListDto>()!;
        expectedBuyingList.CreatedAt = buyingList!.CreatedAt;
        buyingList.Should().BeEquivalentTo(expectedBuyingList);
    }
}
