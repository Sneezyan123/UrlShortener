using System.ComponentModel.DataAnnotations.Schema;

namespace UrlShortener.Domain.Primitives;

public abstract class AggregateRoot : Entity
{
    [NotMapped]
    private readonly List<IDomainEvent> _domainEvents = [];
    [NotMapped]
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    public void RaiseEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public void ClearEvents()
    {
        _domainEvents.Clear();
    }
}