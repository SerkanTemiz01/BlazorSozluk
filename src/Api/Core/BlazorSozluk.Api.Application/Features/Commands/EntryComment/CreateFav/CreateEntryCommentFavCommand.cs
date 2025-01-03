﻿using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorSozluk.Api.Application.Features.Commands.EntryComment.CreateFav
{
    public class CreateEntryCommentFavCommand:IRequest<bool>
    {
        public Guid EntryCommnetId { get; set; }
        public Guid? UserId { get; set; }

        public CreateEntryCommentFavCommand(Guid entryCommnetId, Guid userId)
        {
            EntryCommnetId = entryCommnetId;
            UserId = userId;
        }
    }
}
