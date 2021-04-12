using Application.ActionResult;
using Application.Common;
using Dapper;
using Data.Context;
using Data.UnitOfWork;
using Domain.Entities;
using Domain.Interface;
using MediatR;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.ExampleFeature.Queries
{
    public interface IExampleQuery
    {
        Task<JsonResponse> List(Search search);
        Task<JsonResponse> Get(int id);
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
        public async Task<JsonResponse> Get(int id)
        {
            var data = await _uow.exampleRepository.QuerySingleAsync<ExampleDto>($"select * from ExampleModel where Id = {id}");
            return new JsonResponse(data: data);
        }

        public async Task<JsonResponse> List(Search search)
        {
            #region Query
            var data = await _uow.exampleRepository.QueryAsync<ExampleDto>($"select * from ExampleModel");
            //var data = await _uow.exampleRepository.QueryAsync<ExampleDto>($"select * from ExampleModel where Name like = '%{search.Keyword}%'");
            return data.Sort<ExampleDto>(search);
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
