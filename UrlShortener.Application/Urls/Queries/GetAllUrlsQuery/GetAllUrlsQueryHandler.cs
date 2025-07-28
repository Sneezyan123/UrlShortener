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
        const string query = """
                             SELECT "ShortenedUrlId",
                               "OriginalUrl",
                               "ShortCode",
                               "CreatedAt"
                             FROM public."ShortenedUrls"
                             """;



        var reader = await _db.QueryMultipleAsync(query);
        var result = await reader.ReadAsync<GetUrlDto>();
        return Result.Ok(result);
    }
}