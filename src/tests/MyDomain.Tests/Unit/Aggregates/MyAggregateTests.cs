using AutoFixture.Xunit2;

using MyDomain.Domain.MyAggregate;
using MyDomain.Domain.MyAggregate.Events;

using Shouldly;

namespace MyDomain.Tests;

public class MyAggregateTests
{
    [Theory]
    [AutoData]
    public void GivenAggregateDoesNotExist_WhenCreatingNewAggregate_ThenShouldReturnAggregate(
        string name,
        string description,
        DateTime createdOn)
    {
        // Act
        var aggregate = MyAggregate.Create(name, description, createdOn);

        // Assert
        aggregate.ShouldNotBeNull();
        aggregate.IsNew.ShouldBeTrue();
        aggregate.Id.Value.ShouldNotBe(Guid.Empty);
        aggregate.Version.ShouldBe(1);
        aggregate.State.Name.ShouldBe(name);
        aggregate.State.Description.ShouldBe(description);
        aggregate.State.CreatedOn.ShouldBe(createdOn);
        aggregate.DomainEvents.Count.ShouldBe(1);
        aggregate.DomainEvents[0].ShouldBeOfType<MyAggregateCreated>();
    }

    [Theory]
    [AutoData]
    public void GivenAggregateExists_WhenIUpdateAggregate_ThenAggregateShouldBeUpdatedSuccessfully(
        MyAggregate aggregate,
        string name,
        string description,
        DateTime updatedOn)
    {
        // Arrange
        var expectedVersion = aggregate.Version + 1;

        // Act
        aggregate.Update(name, description, updatedOn);

        // Assert
        aggregate.IsNew.ShouldBeFalse();
        aggregate.Version.ShouldBe(expectedVersion);
        aggregate.State.Name.ShouldBe(name);
        aggregate.State.Description.ShouldBe(description);
        aggregate.State.UpdatedOn.ShouldBe(updatedOn);
        aggregate.DomainEvents.Count.ShouldBe(1);
        aggregate.DomainEvents[0].ShouldBeOfType<MyAggregateUpdated>();
    }
}