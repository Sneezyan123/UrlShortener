// using MediatR;
// using System;
// using System.Threading;
// using System.Threading.Tasks;
// using UrlShortener.Application.DTOs;
// using UrlShortener.Domain.Entities;
// using UrlShortener.Domain.Interfaces;
//
// namespace UrlShortener.Application.Urls.Commands.Handlers
// {
//     public class CreateShortenedUrlCommandHandler : IRequestHandler<CreateShortenedUrlCommand, ShortenedUrlDto>
//     {
//         private readonly IUrlRepository _urlRepository;
//
//         public CreateShortenedUrlCommandHandler(IUrlRepository urlRepository)
//         {
//             _urlRepository = urlRepository;
//         }
//
//         public async Task<ShortenedUrlDto> Handle(CreateShortenedUrlCommand request, CancellationToken cancellationToken)
//         {
//             // Check if URL already exists
//             var existingUrl = await _urlRepository.GetByOriginalUrlAsync(request.OriginalUrl);
//             if (existingUrl != null)
//             {
//                 throw new Exception("This URL has already been shortened");
//             }
//
//             var shortenedUrl = new ShortenedUrl
//             {
//                 Id = Guid.NewGuid(),
//                 OriginalUrl = request.OriginalUrl,
//                 ShortUrl = GenerateShortUrl(),
//                 CreatedDate = DateTime.UtcNow,
//                 CreatedBy = request.Username,
//                 CreatedById = request.UserId
//             };
//
//             await _urlRepository.AddAsync(shortenedUrl);
//
//             return new ShortenedUrlDto
//             {
//                 Id = shortenedUrl.Id,
//                 OriginalUrl = shortenedUrl.OriginalUrl,
//                 ShortUrl = shortenedUrl.ShortUrl,
//                 CreatedDate = shortenedUrl.CreatedDate,
//                 CreatedBy = shortenedUrl.CreatedBy
//             };
//         }
//
//         private string GenerateShortUrl()
//         {
//             // Simple implementation - in real world, you might want to use a more sophisticated algorithm
//             return Convert.ToBase64String(Guid.NewGuid().ToByteArray()).Substring(0, 8);
//         }
//     }
//
//     public class DeleteShortenedUrlCommandHandler : IRequestHandler<DeleteShortenedUrlCommand, bool>
//     {
//         private readonly IUrlRepository _urlRepository;
//
//         public DeleteShortenedUrlCommandHandler(IUrlRepository urlRepository)
//         {
//             _urlRepository = urlRepository;
//         }
//
//         public async Task<bool> Handle(DeleteShortenedUrlCommand request, CancellationToken cancellationToken)
//         {
//             var url = await _urlRepository.GetByIdAsync(request.Id);
//             if (url == null)
//                 return false;
//
//             // Check if user has permission to delete
//             if (!request.IsAdmin && url.CreatedById != request.UserId)
//                 throw new UnauthorizedAccessException("You don't have permission to delete this URL");
//
//             await _urlRepository.DeleteAsync(request.Id);
//             return true;
//         }
//     }
// }
