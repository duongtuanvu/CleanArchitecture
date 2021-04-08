using Application.ActionResult;
using Application.Common;
using Dapper;
using Data.UnitOfWork;
using Domain.Interface;
using MediatR;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.ExampleFeature.Queries
{
    public interface IExampleQuery
    {
        Task<JsonResponse> List(SearchCommon search);
        Task<JsonResponse> Get(int id);
    }
    public class ExampleQuery : IExampleQuery
    {
        private readonly IUnitOfWork _uow;
        public ExampleQuery(IUnitOfWork uow)
        {
            _uow = uow;
        }
        public async Task<JsonResponse> Get(int id)
        {
            var data = await _uow.exampleRepository.QuerySingleAsync<ExampleDto>($"select * from ExampleModel where Id = {id}");
            return new JsonResponse(data: data);
        }

        public async Task<JsonResponse> List(SearchCommon search)
        {
            #region Query
            var data = await _uow.exampleRepository.QueryAsync<ExampleDto>($"select * from ExampleModel where Name like = '%{search.Keyword}%'");
            var totalRecords = data.Count;
            var totalPages = Convert.ToInt32(Math.Ceiling((double)totalRecords / (double)search.PageSize));
            var paging = new Paging(search.PageNumber, search.PageSize, totalPages, totalRecords);
            return new JsonResponse(data: data, paging: paging);
            #endregion

            #region Stored
            //var dp_params = new DynamicParameters();
            //dp_params.Add("Keyword", search.Keyword);
            ////dp_params.Add("retVal", DbType.String, direction: ParameterDirection.Output);
            //var data = await _uow.exampleRepository.QueryAsync<ExampleDto>("StoredName", dp_params, CommandType.StoredProcedure);
            //var totalRecords = data.Count;
            //var totalPages = Convert.ToInt32(Math.Ceiling((double)totalRecords / (double)search.PageSize));
            //var paging = new Paging(search.PageNumber, search.PageSize, totalPages, totalRecords);
            //return new JsonResponse(data: data, paging: paging);
            #endregion
        }
    }
}
