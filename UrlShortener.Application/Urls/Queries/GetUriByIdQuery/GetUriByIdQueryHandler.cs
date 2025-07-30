using System.Data;
using Dapper;
using FluentResults;
using UrlShortener.Applicationm.Abstractions.Messages;
using UrlShortener.Applicationm.ApplicationErrors;
using UrlShortener.Applicationm.Urls.Dtos;
using UrlShortener.Domain.Entities;

namespace UrlShortener.Applicationm.Urls.Queries.GetUriById
{
    public class GetUriByIdQueryHandler : IQueryHandler<GetUriByIdQuery, GetUrlDto>
    {
        private readonly IDbConnection _db;

        public GetUriByIdQueryHandler(IDbConnection db)
        {
            _db = db;
        }

        public async Task<Result<GetUrlDto>> Handle(GetUriByIdQuery request, CancellationToken cancellationToken)
        {
            const string query = """
                                 SELECT 
                                     su."ShortenedUrlId" as Id,
                                     su."OriginalUrl",
                                     su."ShortCode", 
                                     su."CreatedAt",
                                     u."Email_Value" as CreatorEmail
                                 FROM public."ShortenedUrls" su
                                 LEFT JOIN public."Users" u ON su."CreatorId" = u."UserId"
                                 WHERE su."ShortenedUrlId" = @ShortenedUrlId
                                 """;

            try
            {
                var queryDto = await _db.QueryFirstOrDefaultAsync<GetUrlDto>(query, new { ShortenedUrlId = request.ShortenUrlId });
                
                if (queryDto == null)
                {
                    return Result.Fail(ApplicationError.NotFoundEntityById(nameof(ShortenedUrl), request.ShortenUrlId));
                }

                return Result.Ok(queryDto);
            }
            catch (Exception ex)
            {
                return Result.Fail($"Database error: {ex.Message}");
            }
        }
    }
}