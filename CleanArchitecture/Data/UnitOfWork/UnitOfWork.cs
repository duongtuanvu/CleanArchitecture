using Data.Context;
using Data.Repository;
using Domain.Entities;
using Domain.Interface;
using Microsoft.Extensions.Configuration;
using System;

namespace Data.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private IRepository<ExampleModel> _emxampleRepository;
        private readonly IConfiguration _configuration;
        public UnitOfWork(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public IRepository<ExampleModel> exampleRepository
        {
            get { return _emxampleRepository ??= new Repository<ExampleModel>(_context, _configuration); }
        }

        private bool disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            this.disposed = true;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
