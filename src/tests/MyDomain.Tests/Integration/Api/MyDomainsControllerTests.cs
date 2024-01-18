using System.Net;
using System.Net.Http.Headers;
using System.Text;

using AutoFixture.Xunit2;

using MyDomain.Api.Authorization;
using MyDomain.Contracts.Models.V1;

using Newtonsoft.Json;

using Shouldly;

namespace MyDomain.Tests.Integration.Api;

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
    public async Task GetById_WhenIsAuthenticated_AndMyDomainExists_ThenShouldReturnMyDomain(
        string name)
    {
        // Arrange
        var result = await Given_MyDomainExists(name);
        var request = Given_GetMyDomainByIdRequest(result!.Id);
        Given_RequestIsAuthenticated(request);

        // Act
        var response = await _client.SendAsync(request);
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        Then_Response_ShouldBeOk(response);
        Then_Content_ShouldBeCorrect(content, result.Id, result.Name, result.Description);
    }

    [Theory]
    [AutoData]
    public async Task GetById_WhenIsAuthenticated_AndMyDomainDoesNotExist_ThenShouldReturnNotFound(
        Guid id)
    {
        // Arrange
        var request = Given_GetMyDomainByIdRequest(id);
        Given_RequestIsAuthenticated(request);

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        Then_Response_ShouldBeNotFound(response);
    }

    [Theory]
    [AutoData]
    public async Task GetById_WhenIsNotAuthenticated_ThenShouldReturnUnauthorized(
        Guid id)
    {
        // Arrange
        var request = Given_GetMyDomainByIdRequest(id);

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        Then_Response_ShouldBeUnauthorized(response);
    }

    [Theory]
    [AutoData]
    public async Task Create_WhenIsAuthenticated_ThenShouldReturnCreated(
        string name)
    {
        // Arrange
        var request = Given_CreateDomainRequest(name);
        Given_RequestIsAuthenticated(request);

        // Act
        var response = await _client.SendAsync(request);
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        Then_Response_ShouldBeCreated(response);
        Then_Content_ShouldBeCorrect(content, name);
    }

    [Fact]
    public async Task Create_WhenIsAuthenticated_AndRequestIsMissingRequiredFields_ThenShouldReturnBadRequest()
    {
        // Arrange
        var request = Given_CreateDomainRequest(string.Empty);
        Given_RequestIsAuthenticated(request);

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        Then_Response_ShouldBeBadRequest(response);
    }

    [Theory]
    [AutoData]
    public async Task Update_WhenIsAuthenticated_ThenShouldReturnOk(
        string name,
        string description)
    {
        // Arrange
        var result = await Given_MyDomainExists(name);
        var request = Given_UpdateDomainRequest(result!.Id, name, description);
        Given_RequestIsAuthenticated(request);

        // Act
        var response = await _client.SendAsync(request);
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        Then_Response_ShouldBeOk(response);
        Then_Content_ShouldBeCorrect(content, result.Id, name, description);
    }

    [Theory]
    [AutoData]
    public async Task Update_WhenIsAuthenticated_AndRequestIsMissingRequiredFields_ThenShouldReturnBadRequest(
        Guid id)
    {
        // Arrange
        var request = Given_UpdateDomainRequest(id, string.Empty, string.Empty);
        Given_RequestIsAuthenticated(request);

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

    private HttpRequestMessage Given_UpdateDomainRequest(Guid id, string name, string description)
    {
        var requestUri = new UriBuilder(_baseUrl);
        requestUri.Path += id.ToString();

        return new HttpRequestMessage
        {
            Method = HttpMethod.Put,
            RequestUri = requestUri.Uri,
            Content = new StringContent($"{{\"name\":\"{name}\", \"description\":\"{description}\"}}", Encoding.UTF8, "application/json")
        };
    }

    private void Given_RequestIsAuthenticated(HttpRequestMessage request)
    {
        var token = _fixture.GenerateTokenFor(Scopes.Write);

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    private async Task<MyDomainDto?> Given_MyDomainExists(string name)
    {
        var createRequest = Given_CreateDomainRequest(name);
        Given_RequestIsAuthenticated(createRequest);
        var createResponse = await _client.SendAsync(createRequest);

        var content = await createResponse.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<MyDomainDto>(content);

        return result;
    }

    private static void Then_Response_ShouldBeOk(HttpResponseMessage response)
    {
        response.ShouldNotBeNull();
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    private static void Then_Response_ShouldBeNotFound(HttpResponseMessage response)
    {
        response.ShouldNotBeNull();
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
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

    private static void Then_Response_ShouldBeUnauthorized(HttpResponseMessage response)
    {
        response.ShouldNotBeNull();
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    private static void Then_Content_ShouldBeCorrect(string content, string name)
    {
        var actualResult = JsonConvert.DeserializeObject<MyDomainDto>(content);
        actualResult.ShouldNotBeNull();
        actualResult.Id.ShouldNotBe(Guid.Empty);
        actualResult.Name.ShouldBe(name);
    }

    private static void Then_Content_ShouldBeCorrect(string content, Guid id, string name, string description)
    {
        var actualResult = JsonConvert.DeserializeObject<MyDomainDto>(content);
        actualResult.ShouldNotBeNull();
        actualResult.Id.ShouldBe(id);
        actualResult.Name.ShouldBe(name);
        actualResult.Description.ShouldBe(description);
    }
}