namespace Scrips.Core.Domain.Common.Contracts;

public interface IEntity
{
    List<DomainEvent> DomainEvents { get; }
}

public interface IEntity<TGuid> : IEntity
{
    TGuid Id { get; }
}