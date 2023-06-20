using System.Net;
using System.Text;

using AutoFixture.Xunit2;

using Shouldly;

namespace MyDomain.Tests.Integration.Controllers;

[Collection(nameof(ApiWebApplicationFactoryCollection))]
public class MyDomainsControllerTests
{
    private readonly ApiWebApplicationFactory _fixture;
    private readonly HttpClient _client;
    private readonly string _baseUrl;

    public MyDomainsControllerTests(ApiWebApplicationFactory fixture)
    {
        _fixture = fixture;
        _client = _fixture.CreateClient();
        _baseUrl = $"{_client.BaseAddress}v1/mydomains/";
    }

    [Theory]
    [AutoData]
    public async Task GetById_WhenMyDomainExists_ThenShouldReturnMyDomain(Guid id)
    {
        // Arrange
        var request = Given_GetMyDomainByIdRequest(id);

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        Then_Response_ShouldBeOk(response);
    }

    [Theory]
    [AutoData]
    public async Task Create_WhenMyDomainIsCreatedSuccessfully_ThenShouldReturnCreated(
        string name)
    {
        // Arrange
        var request = Given_CreateDomainRequest(name);

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        Then_Response_ShouldBeCreated(response);
    }

    [Fact]
    public async Task Create_WhenRequestIsMissingRequiredFields_ThenShouldReturnBadRequest()
    {
        // Arrange
        var request = Given_CreateDomainRequest(string.Empty);

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        Then_Response_ShouldBeBadRequest(response);
    }

    [Theory]
    [AutoData]
    public async Task Update_WhenMyDomainIsUpdatedSuccessfully_ThenShouldReturnOk(
        Guid id,
        string name)
    {
        // Arrange
        var request = Given_UpdateDomainRequest(id, name);

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        Then_Response_ShouldBeOk(response);
    }

    [Theory]
    [AutoData]
    public async Task Update_WhenRequestIsMissingRequiredFields_ThenShouldReturnBadRequest(
        Guid id)
    {
        // Arrange
        var request = Given_UpdateDomainRequest(id, string.Empty);

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        Then_Response_ShouldBeBadRequest(response);
    }       

    private HttpRequestMessage Given_GetMyDomainByIdRequest(Guid id)
    {
        var requestUri = new UriBuilder(_baseUrl);
        requestUri.Path += id.ToString();

        return new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = requestUri.Uri
        };
    }

    private HttpRequestMessage Given_CreateDomainRequest(string name)
    {
        var requestUri = new UriBuilder(_baseUrl);

        return new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = requestUri.Uri,
            Content = new StringContent($"{{\"name\":\"{name}\"}}", Encoding.UTF8, "application/json")
        };
    }

    private HttpRequestMessage Given_UpdateDomainRequest(Guid id, string name)
    {
        var requestUri = new UriBuilder(_baseUrl);
        requestUri.Path += id.ToString();

        return new HttpRequestMessage
        {
            Method = HttpMethod.Put,
            RequestUri = requestUri.Uri,
            Content = new StringContent($"{{\"name\":\"{name}\"}}", Encoding.UTF8, "application/json")
        };
    }    

    private static void Then_Response_ShouldBeOk(HttpResponseMessage response)
    {
        response.ShouldNotBeNull();
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    private static void Then_Response_ShouldBeCreated(HttpResponseMessage response)
    {
        response.ShouldNotBeNull();
        response.StatusCode.ShouldBe(HttpStatusCode.Created);
    }

    private static void Then_Response_ShouldBeBadRequest(HttpResponseMessage response)
    {
        response.ShouldNotBeNull();
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }    
}