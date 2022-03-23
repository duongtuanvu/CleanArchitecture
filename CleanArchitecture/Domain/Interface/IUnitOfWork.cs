using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interface
{
    public interface IUnitOfWork : IDisposable
    {
        public IRepository<ExampleModel> ExampleRepository { get; }
        Task SaveChangeAsync();
    }
}
