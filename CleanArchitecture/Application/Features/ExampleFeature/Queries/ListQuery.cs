using Application.Extensions;
using AutoMapper;
using Data.Context;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.ExampleFeature.Queries
{
    public class ListQuery : IRequest<ResponseExtension>
    {
        public ListQuery(SearchExtension search)
        {
            Search = search;
        }
        public SearchExtension Search { get; set; }
        public class ListQueryHandler : IRequestHandler<ListQuery, ResponseExtension>
        {
            private readonly ApplicationDbContext _context;
            private readonly IMapper _mapper;
            public ListQueryHandler(ApplicationDbContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }
            public async Task<ResponseExtension> Handle(ListQuery request, CancellationToken cancellationToken)
            {
                var response = _context.ExampleModel.AsNoTracking().Sort<ExampleModel>(request.Search);
                response.Data = ((IEnumerable<ExampleModel>)response.Data).Select(x => _mapper.Map<ExampleDto>(x));
                return response;
            }
        }
    }
}
