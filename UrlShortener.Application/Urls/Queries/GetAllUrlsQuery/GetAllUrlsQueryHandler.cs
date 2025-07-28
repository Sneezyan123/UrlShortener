using System.Data;
using Dapper;
using FluentResults;
using UrlShortener.Applicationm.Abstractions.Messages;
using UrlShortener.Applicationm.Urls.Dtos;

namespace UrlShortener.Applicationm.Urls.Queries.GetAllUrlsQuery;

public class GetAllUrlsQueryHandler: IQueryHandler<GetAllUrlsQuery, IEnumerable<GetUrlDto>>
{
    private readonly IDbConnection _db;
    public GetAllUrlsQueryHandler(IDbConnection db)
    {
        _db = db;
    }
    public async Task<Result<IEnumerable<GetUrlDto>>> Handle(GetAllUrlsQuery request, CancellationToken cancellationToken)
    {
        var query = """
                        SELECT urls."Id", urls."OriginalUrl", urls."ShortCode", urls."CreatedAt", users."Email_Value" AS "CreatorEmail"
                        FROM public."ShortenedUrls" AS urls
                        JOIN public."Users" AS users ON urls."CreatorId" = users."UserId"
                        WHERE urls."CreatorId" = @UserId;
                    """;

        var reader = await _db.QueryMultipleAsync(query);
        var result = await reader.ReadAsync<GetUrlDto>();
        return Result.Ok(result);
    }
}