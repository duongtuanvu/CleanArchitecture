using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Interface
{
    public interface IUnitOfWork : IDisposable
    {
        public IRepository<ExampleModel> exampleRepository { get; }
    }
}
