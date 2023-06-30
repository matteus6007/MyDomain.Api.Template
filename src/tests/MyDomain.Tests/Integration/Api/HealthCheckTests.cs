using System.Net;

using Shouldly;

namespace MyDomain.Tests.Integration.Api;

[Collection(nameof(ApiWebApplicationFactoryCollection))]
public class HealthCheckTests
{
    private readonly ApiWebApplicationFactory _fixture;
    private readonly HttpClient _client;
    private readonly string _baseUrl;

    public HealthCheckTests(ApiWebApplicationFactory fixture)
    {
        _fixture = fixture;
        _client = _fixture.CreateClient();
        _baseUrl = $"{_client.BaseAddress}";
    }

    [Fact]
    public async Task WhenICallHealthCheck_ThenShouldReturnOK()
    {
        // Arrange
        var request = Given_HealthCheckRequest();

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        Then_Response_ShouldBeOk(response);
    }

    private HttpRequestMessage Given_HealthCheckRequest()
    {
        var requestUri = new UriBuilder(_baseUrl);
        requestUri.Path += "healthcheck/tests";

        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = requestUri.Uri
        };
        return request;
    }

    private static void Then_Response_ShouldBeOk(HttpResponseMessage response)
    {
        response.ShouldNotBeNull();
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }
}