using FluentResults;

namespace UrlShortener.Applicationm.ApplicationErrors;

public static class ApplicationError
{
    public static Error NotFoundEntityById(string entityName, Guid id) => new Error($"{entityName} with id:{id} was not found!");

}