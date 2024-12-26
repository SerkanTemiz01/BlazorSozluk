using BlazorSozluk.Api.Application.Interfaces.Repositories;
using BlazorSozluk.Common.Infrastructure.Extensions;
using BlazorSozluk.Common.Models.Page;
using BlazorSozluk.Common.Models.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorSozluk.Api.Application.Features.Queries.GetEntryComments
{
    public class GetEntryCommentsQueryHandler : IRequestHandler<GetEntryCommentsQuery, PagedViewModel<GetEntryCommentViewModel>>
    {
        private readonly IEntryCommentRepository entryCommentRepository;

        public GetEntryCommentsQueryHandler(IEntryCommentRepository entryCommentRepository)
        {
            this.entryCommentRepository = entryCommentRepository;
        }

        public async Task<PagedViewModel<GetEntryCommentViewModel>> Handle(GetEntryCommentsQuery request, CancellationToken cancellationToken)
        {
            var query = entryCommentRepository.AsQueryable();

            query = query.Include(x => x.EntryCommentFavorites)
                .Include(x => x.CreatedBy)
                .Include(x => x.EntryCommentVotes)
                .Where(x=>x.EntryId==request.EntryId);

            var list = query.Select(x => new GetEntryCommentViewModel()
            {
                Id = x.Id,             
                Content = x.Content,
                IsFavorited = request.UserId.HasValue && x.EntryCommentFavorites.Any(y => y.CreatedById == request.UserId),
                FavoritedCount = x.EntryCommentFavorites.Count(),
                CreatedDate = x.CreateDate,
                CreatedByUserName = x.CreatedBy.UserName,
                VoteType =
                request.UserId.HasValue && x.EntryCommentVotes.Any(y => y.CreatedById == request.UserId)
                ? x.EntryCommentVotes.FirstOrDefault(y => y.CreatedById == request.UserId).VoteType
                : Common.ViewModels.VoteType.None,
            });

            var entries = await list.GetPaged(request.Page, request.PageSize);

            return entries;
        }
    }
}
