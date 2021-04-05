using Data.Context;
using Data.Repository;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private IRepository<ExampleModel> _emxampleRepository;
        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        public IRepository<ExampleModel> exampleRepository
        {
            get { return _emxampleRepository ??= new Repository<ExampleModel>(_context); }
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
