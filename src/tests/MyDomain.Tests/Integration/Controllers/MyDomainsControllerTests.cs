using System.Net;

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

    private static void Then_Response_ShouldBeOk(HttpResponseMessage response)
    {
        response.ShouldNotBeNull();
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    } 
}