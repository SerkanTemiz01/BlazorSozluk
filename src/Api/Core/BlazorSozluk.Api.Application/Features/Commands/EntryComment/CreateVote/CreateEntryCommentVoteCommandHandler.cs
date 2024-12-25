using BlazorSozluk.Common;
using BlazorSozluk.Common.Events.EntryComment;
using BlazorSozluk.Common.Infrastructure;
using BlazorSozluk.Common.Models.RequestModels;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorSozluk.Api.Application.Features.Commands.EntryComment.CreateVote
{
    public class CreateEntryCommentVoteCommandHandler : IRequestHandler<CreateEntryCommentVoteCommand, bool>
    {
        public async Task<bool> Handle(CreateEntryCommentVoteCommand request, CancellationToken cancellationToken)
        {
            QueueFactory.SendMessageToExchange(exchangeName: SozlukConstants.VoteExchangeName,
                                               exchangeType: SozlukConstants.DefaultExchangeType,
                                               queueName: SozlukConstants.CreateEntryCommentVoteQueueName,
                                               obj: new CreateEntryCommentVoteEvent()
                                               {
                                                   CreatedBy = request.CreatedBy,
                                                   VoteType = request.VoteType,
                                                   EntryCommentId = request.EntryCommentId
                                               });
            return await Task.FromResult(true);
        }
    }
}
