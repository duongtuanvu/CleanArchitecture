using ExampleService.Infrastructure.Entities;
using ExampleService.Infrastructure.Interface.IRepository;
using ExampleService.Infrastructure.Interface.IRepository.ExampleRepository;
using ExampleService.Infrastructure.Interface.Repository;
using ExampleService.Infrastructure.Interface.Repository.ExampleRepository;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExampleService.Infrastructure.Interface.UnitOfWork
{
    public interface IUnitOfWork
    {
        public IRepository<Example> ExampleRepository { get; }
        public IUserRepository UserRepository { get; }
        public IExampleRepository ExampleRepo { get; }
    }
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly ApplicationDbContext _context;
        private IRepository<Example> _emxampleRepository;
        private IUserRepository _userRepository;
        public IExampleRepository _exampleRepo;
        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        public IUserRepository UserRepository
        {
            get { return _userRepository ??= new UserRepository(_context); }
        }

        public IRepository<Example> ExampleRepository
        {
            get { return _emxampleRepository ??= new Repository<Example>(_context); }
        }

        public IExampleRepository ExampleRepo
        {
            get { return _exampleRepo ??= new ExampleRepository(_context); }
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
