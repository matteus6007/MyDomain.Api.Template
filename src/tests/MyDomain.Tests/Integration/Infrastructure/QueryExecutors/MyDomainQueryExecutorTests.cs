using AutoFixture.Xunit2;

using Microsoft.Extensions.Options;

using MyDomain.Domain.Models;
using MyDomain.Infrastructure.Persistence.Options;
using MyDomain.Infrastructure.Persistence.QueryExecutors;

using NSubstitute;

using Shouldly;

namespace MyDomain.Tests.Integration.Infrastructure.QueryExecutors;

public class MyDomainQueryExecutorTests : IDisposable
{
    private readonly DatabaseHelper<Guid, MyDomainReadModel> _databaseHelper;
    private readonly MyDomainQueryExecutor _sut;

    public MyDomainQueryExecutorTests()
    {
        _databaseHelper = new DatabaseHelper<Guid, MyDomainReadModel>("MyAggregates", "Id");

        var options = Substitute.For<IOptionsSnapshot<DatabaseOptions>>();
        options.Value.Returns(_databaseHelper.Options);

        _sut = new MyDomainQueryExecutor(options);
    }

    public void Dispose() => _databaseHelper.CleanTableAsync().GetAwaiter().GetResult();

    [Theory]
    [AutoData]
    public async Task GetByIdAsync_WhenRecordExists_ThenShouldReturnRecord(
        MyDomainReadModel model)
    {
        // Arrange
        await GivenRecordExists(model);

        // Act
        var result = await _sut.GetByIdAsync(model.Id);

        // Assert
        result.ShouldNotBeNull();
        result.Name.ShouldBe(model.Name);
        result.CreatedOn.ShouldBe(model.CreatedOn, TimeSpan.FromSeconds(1));
        result.UpdatedOn.ShouldBe(model.UpdatedOn, TimeSpan.FromSeconds(1));
    }

    private async Task GivenRecordExists(MyDomainReadModel model)
    {
        await _databaseHelper.AddRecordAsync(model.Id, model);
    }
}