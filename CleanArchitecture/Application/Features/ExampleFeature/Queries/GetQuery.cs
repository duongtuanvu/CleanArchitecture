using Application.Extensions;
using AutoMapper;
using Data.Context;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.ExampleFeature.Queries
{
    public class GetQuery : IRequest<ResponseExtension>
    {
        public GetQuery(int id)
        {
            Id = id;
        }
        public int Id { get; set; }
        public class GetQueryHandler : IRequestHandler<GetQuery, ResponseExtension>
        {
            private readonly ApplicationDbContext _context;
            private readonly IMapper _mapper;
            public GetQueryHandler(ApplicationDbContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<ResponseExtension> Handle(GetQuery request, CancellationToken cancellationToken)
            {
                var example = await _context.ExampleModel.FindAsync(request.Id);
                var dto = _mapper.Map<ExampleDto>(example);
                return new ResponseExtension(data: dto);
            }
        }
    }
}
