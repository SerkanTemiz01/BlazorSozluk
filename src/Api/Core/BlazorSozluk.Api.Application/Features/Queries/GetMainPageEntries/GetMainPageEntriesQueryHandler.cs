using BlazorSozluk.Api.Application.Interfaces.Repositories;
using BlazorSozluk.Common.Infrastructure.Extensions;
using BlazorSozluk.Common.Models.Page;
using BlazorSozluk.Common.Models.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BlazorSozluk.Api.Application.Features.Queries.GetMainPageEntries
{
    public class GetMainPageEntriesQueryHandler : IRequestHandler<GetMainPageEntriesQuery, PagedViewModel<GetEntryDetailViewModel>>
    {

        private readonly IEntryRepository entryRepository;


        public GetMainPageEntriesQueryHandler(IEntryRepository entryRepository)
        {
            this.entryRepository = entryRepository;         
        }
        public async Task<PagedViewModel<GetEntryDetailViewModel>> Handle(GetMainPageEntriesQuery request, CancellationToken cancellationToken)
        {
            var query = entryRepository.AsQueryable();

            query = query.Include(x => x.EntryFavorites)
                .Include(x=>x.CreatedBy)
                .Include(x => x.EntryVotes);

            var list = query.Select(x => new GetEntryDetailViewModel()
            {
                Id = x.Id,
                Subject = x.Subject,
                Content = x.Content,
                IsFavorited = request.UserId.HasValue && x.EntryFavorites.Any(y => y.CreatedById == request.UserId),
                FavoritedCount = x.EntryFavorites.Count(),
                CreatedDate = x.CreateDate,
                CreatedByUserName = x.CreatedBy.UserName,
                VoteType=
                request.UserId.HasValue && x.EntryVotes.Any(y=> y.CreatedById == request.UserId)
                ? x.EntryVotes.FirstOrDefault(y=>y.CreatedById == request.UserId).VoteType 
                : Common.ViewModels.VoteType.None,
            });
            
            var entries= await list.GetPaged(request.Page, request.PageSize);

            return entries;
        }
    }
}
