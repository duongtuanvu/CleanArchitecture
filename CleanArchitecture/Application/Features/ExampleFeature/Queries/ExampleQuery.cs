using Data.UnitOfWork;
using Domain.Interface;
using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.ExampleFeature.Queries
{
    public interface IExampleQuery
    {
        Task<IEnumerable<ExampleDto>> List();
        Task<ExampleDto> Get(int id);
    }
    public class ExampleQuery : IExampleQuery
    {
        private readonly IUnitOfWork _uow;
        public ExampleQuery(IUnitOfWork uow)
        {
            _uow = uow;
        }
        public async Task<ExampleDto> Get(int id)
        {
            return await _uow.exampleRepository.QuerySingleAsync<ExampleDto>($"select * from ExampleModel where Id = {id}");
        }

        public Task<IEnumerable<ExampleDto>> List()
        {
            throw new NotImplementedException();
        }
    }
}
