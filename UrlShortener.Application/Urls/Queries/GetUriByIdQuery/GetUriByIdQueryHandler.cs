using System.Data;
using Dapper;
using FluentResults;
using UrlShortener.Applicationm.Abstractions.Messages;
using UrlShortener.Applicationm.ApplicationErrors;
using UrlShortener.Applicationm.Urls.Dtos;
using UrlShortener.Domain.Entities;

namespace UrlShortener.Applicationm.Urls.Queries.GetUriById;

public class GetUriByIdQueryHandler: IQueryHandler<GetUriByIdQuery, GetUrlDto>
{
    private readonly IDbConnection _db;
    public GetUriByIdQueryHandler(IDbConnection db)
    {
        _db = db;
    }
    public async Task<Result<GetUrlDto>> Handle(GetUriByIdQuery request, CancellationToken cancellationToken)
    {
        var query = """
                    SELECT urls."ShortenedUrlId", urls."OriginalUrl", urls."ShortCode", urls."CreatedAt"
                    from public."ShortenUrls" as urls
                    where urls."Id" = @ShortenUrlId;
                    """;
        var queryDto = await _db.QueryFirstOrDefaultAsync<GetUrlDto>(query, new { ShortenedUrlId = request.ShortenUrlId });
        if (queryDto is null) return Result.Fail(ApplicationError.NotFoundEntityById(nameof(ShortenedUrl), request.ShortenUrlId));

        return Result.Ok(queryDto);
    }
}