using AutoFixture.Xunit2;

using MyDomain.Domain.MyDomainAggregate;
using MyDomain.Domain.MyDomainAggregate.Events;

using Shouldly;

namespace MyDomain.Tests;

public class MyDomainAggregateTests
{
    [Theory]
    [AutoData]
    public void GivenAggregateDoesNotExist_WhenCreatingNewAggregate_ThenShouldReturnAggregate(
        string name,
        string description,
        DateTime createdOn)
    {
        // Act
        var aggregate = MyDomainAggregate.Create(name, description, createdOn);

        // Assert
        aggregate.ShouldNotBeNull();
        aggregate.IsNew.ShouldBeTrue();
        aggregate.Id.Value.ShouldNotBe(Guid.Empty);
        aggregate.Version.ShouldBe(1);
        aggregate.State.Name.ShouldBe(name);
        aggregate.State.Description.ShouldBe(description);
        aggregate.State.CreatedOn.ShouldBe(createdOn);
        aggregate.DomainEvents.Count.ShouldBe(1);
        aggregate.DomainEvents[0].ShouldBeOfType<MyDomainCreated>();
    }

    [Theory]
    [AutoData]
    public void GivenAggregateExists_WhenIUpdateAggregate_ThenAggregateShouldBeUpdatedSuccessfully(
        MyDomainAggregate aggregate,
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
        aggregate.DomainEvents[0].ShouldBeOfType<MyDomainUpdated>();
    }
}