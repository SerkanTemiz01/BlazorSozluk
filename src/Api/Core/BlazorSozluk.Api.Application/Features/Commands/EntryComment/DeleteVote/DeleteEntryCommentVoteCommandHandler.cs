using BlazorSozluk.Common;
using BlazorSozluk.Common.Events.EntryComment;
using BlazorSozluk.Common.Infrastructure;
using MediatR;

namespace BlazorSozluk.Api.Application.Features.Commands.EntryComment.DeleteVote
{
    public class DeleteEntryCommentVoteCommandHandler : IRequestHandler<DeleteEntryCommentVoteCommand, bool>
    {
        public Task<bool> Handle(DeleteEntryCommentVoteCommand request, CancellationToken cancellationToken)
        {
            QueueFactory.SendMessageToExchange(exchangeName: SozlukConstants.VoteExchangeName,
                                               exchangeType: SozlukConstants.DefaultExchangeType,
                                               queueName: SozlukConstants.DeleteEntryCommentVoteQueueName,
                                               obj: new DeleteEntryCommentVoteEvent()
                                               {
                                                   EntryCommentId = request.EntryCommentId,
                                                   CreatedBy = request.UserId
                                               });
            return Task.FromResult(true);
        }
    }
}
