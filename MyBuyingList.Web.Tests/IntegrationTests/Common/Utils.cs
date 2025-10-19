using MyBuyingList.Application.Features.Users.DTOs;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace MyBuyingList.Web.Tests.IntegrationTests.Common;

public static class Utils
{
    private const string TestUserEmail = "newemail@gmail.com";
    public const string TestUserPassword = "Mx485!@zz";
    public const string TestUserUsername = "test_user";

    public static async Task<int> InsertTestUser(HttpClient client)
    {
        var newUser = new CreateUserDto(TestUserUsername, TestUserEmail, TestUserPassword);
                
        var response = await client.PostAsync(
            requestUri: Constants.BaseAddressUserEndpoint, 
            content: Utils.GetJsonContentFromObject(newUser)
        );

        if (response.StatusCode != HttpStatusCode.Created)
        {
            throw new Exception("Could not create test user.");
        }

        var content = await response.Content.ReadFromJsonAsync<int>();
        return content;
    }

    public static StringContent GetJsonContentFromObject(object content)
    {
        var jsonBody = JsonSerializer.Serialize(content);
        return new StringContent(jsonBody, Encoding.UTF8, "application/json");
    }
}
