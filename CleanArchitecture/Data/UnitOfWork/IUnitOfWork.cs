using Data.Repository;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        public IRepository<ExampleModel> exampleRepository { get; }
    }
}
