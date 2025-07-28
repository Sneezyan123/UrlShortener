// using MediatR;
// using System.Collections.Generic;
// using System.Linq;
// using System.Threading;
// using System.Threading.Tasks;
// using UrlShortener.Application.DTOs;
// using UrlShortener.Domain.Interfaces;
//
// namespace UrlShortener.Application.Urls.Queries.Handlers
// {
//     public class GetAllUrlsQueryHandler : IRequestHandler<GetAllUrlsQuery, IEnumerable<ShortenedUrlDto>>
//     {
//         private readonly IUrlRepository _urlRepository;
//
//         public GetAllUrlsQueryHandler(IUrlRepository urlRepository)
//         {
//             _urlRepository = urlRepository;
//         }
//
//         public async Task<IEnumerable<ShortenedUrlDto>> Handle(GetAllUrlsQuery request, CancellationToken cancellationToken)
//         {
//             var urls = await _urlRepository.GetAllAsync();
//             return urls.Select(u => new ShortenedUrlDto
//             {
//                 Id = u.Id,
//                 OriginalUrl = u.OriginalUrl,
//                 ShortUrl = u.ShortUrl,
//                 CreatedDate = u.CreatedDate,
//                 CreatedBy = u.CreatedBy
//             });
//         }
//     }
//
//     public class GetUrlByIdQueryHandler : IRequestHandler<GetUrlByIdQuery, ShortenedUrlDto>
//     {
//         private readonly IUrlRepository _urlRepository;
//
//         public GetUrlByIdQueryHandler(IUrlRepository urlRepository)
//         {
//             _urlRepository = urlRepository;
//         }
//
//         public async Task<ShortenedUrlDto> Handle(GetUrlByIdQuery request, CancellationToken cancellationToken)
//         {
//             var url = await _urlRepository.GetByIdAsync(request.Id);
//             if (url == null) return null;
//
//             return new ShortenedUrlDto
//             {
//                 Id = url.Id,
//                 OriginalUrl = url.OriginalUrl,
//                 ShortUrl = url.ShortUrl,
//                 CreatedDate = url.CreatedDate,
//                 CreatedBy = url.CreatedBy
//             };
//         }
//     }
//
//     public class GetUserUrlsQueryHandler : IRequestHandler<GetUserUrlsQuery, IEnumerable<ShortenedUrlDto>>
//     {
//         private readonly IUrlRepository _urlRepository;
//
//         public GetUserUrlsQueryHandler(IUrlRepository urlRepository)
//         {
//             _urlRepository = urlRepository;
//         }
//
//         public async Task<IEnumerable<ShortenedUrlDto>> Handle(GetUserUrlsQuery request, CancellationToken cancellationToken)
//         {
//             var urls = await _urlRepository.GetByUserIdAsync(request.UserId);
//             return urls.Select(u => new ShortenedUrlDto
//             {
//                 Id = u.Id,
//                 OriginalUrl = u.OriginalUrl,
//                 ShortUrl = u.ShortUrl,
//                 CreatedDate = u.CreatedDate,
//                 CreatedBy = u.CreatedBy
//             });
//         }
//     }
// }
