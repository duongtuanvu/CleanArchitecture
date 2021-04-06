using Data.Repository;
using Domain.Entities;
using System;

namespace Data.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        public IRepository<ExampleModel> exampleRepository { get; }
    }
}
