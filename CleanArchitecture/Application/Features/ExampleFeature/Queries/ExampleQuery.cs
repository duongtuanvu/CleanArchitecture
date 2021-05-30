using Application.Extensions;
using Application.Extensions;
using Data.Context;
using Domain.Interface;
using System.Threading.Tasks;

namespace Application.Features.ExampleFeature.Queries
{
    public interface IExampleQuery
    {
        Task<ResponseExtension> List(SearchBase search);
        Task<ResponseExtension> Get(int id);
    }
    public class ExampleQuery : IExampleQuery
    {
        private readonly IUnitOfWork _uow;
        private readonly ApplicationDbContext _context;
        public ExampleQuery(IUnitOfWork uow, ApplicationDbContext context)
        {
            _context = context;
            _uow = uow;
        }
        public async Task<ResponseExtension> Get(int id)
        {
            var data = await _uow.exampleRepository.QuerySingleAsync<ExampleDto>($"select * from ExampleModel where Id = {id}");
            return new ResponseExtension(data: data);
        }
        public async Task<ResponseExtension> List(SearchBase search)
        {
            #region Query
            var data = await _uow.exampleRepository.QueryAsync<ExampleDto>($"select * from ExampleModel");
            //var data = await _uow.exampleRepository.QueryAsync<ExampleDto>($"select * from ExampleModel where Name like = '%{search.Keyword}%'");
            return await data.Sort<ExampleDto>(search);
            #endregion

            #region StoredProcedure
            //var dp_params = new DynamicParameters();
            //dp_params.Add("Keyword", search.Keyword);
            ////dp_params.Add("retVal", DbType.String, direction: ParameterDirection.Output);
            //var data = await _uow.exampleRepository.QueryAsync<ExampleDto>("StoredName", dp_params, CommandType.StoredProcedure);
            //return data.Sort<ExampleDto>(search);
            #endregion
        }
    }
}
