using System.Data;
using Dapper;
using FluentResults;
using UrlShortener.Applicationm.Abstractions.Messages;
using UrlShortener.Applicationm.Urls.Dtos;

namespace UrlShortener.Applicationm.Urls.Queries.GetAllUrlsQuery
{
    public class GetAllUrlsQueryHandler : IQueryHandler<GetAllUrlsQuery, IEnumerable<GetUrlDto>>
    {
        private readonly IDbConnection _db;

        public GetAllUrlsQueryHandler(IDbConnection db)
        {
            _db = db;
        }

        public async Task<Result<IEnumerable<GetUrlDto>>> Handle(GetAllUrlsQuery request, CancellationToken cancellationToken)
        {
            const string query = """
                                 SELECT 
                                     su."ShortenedUrlId" as Id,
                                     su."OriginalUrl",
                                     su."ShortCode",
                                     su."CreatedAt",
                                     u."Email_Value" as CreatorEmail
                                 FROM "ShortenedUrls" su
                                 LEFT JOIN "Users" u ON su."CreatorId" = u."UserId"
                                 ORDER BY su."CreatedAt" DESC
                                 LIMIT 50
                                 """;

            try
            {
                var result = await _db.QueryAsync<GetUrlDto>(query);
                return Result.Ok(result);
            }
            catch (Exception ex)
            {
                return Result.Fail($"Database error: {ex.Message}");
            }
        }
    }
}