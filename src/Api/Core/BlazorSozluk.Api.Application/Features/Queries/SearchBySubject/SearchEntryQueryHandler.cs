﻿using BlazorSozluk.Api.Application.Interfaces.Repositories;
using BlazorSozluk.Common.Models.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorSozluk.Api.Application.Features.Queries.SearchBySubject
{
    public class SearchEntryQueryHandler : IRequestHandler<SearchEntryQuery, List<SearchEntryViewModel>>
    {
        private readonly IEntryRepository entryRepository;

        public SearchEntryQueryHandler(IEntryRepository entryRepository)
        {
            this.entryRepository = entryRepository;
        }

        public async Task<List<SearchEntryViewModel>> Handle(SearchEntryQuery request, CancellationToken cancellationToken)
        {
            //TODO validation, request.SearchText length should be checked 

            var result = entryRepository
                .Get(x => EF.Functions.Like(x.Subject, $"{request.SearchText}%"))
                .Select(x => new SearchEntryViewModel()
                {
                    Id = x.Id,
                    Subject = x.Subject,
                });
            return await result.ToListAsync(cancellationToken);
        }
    }
}
