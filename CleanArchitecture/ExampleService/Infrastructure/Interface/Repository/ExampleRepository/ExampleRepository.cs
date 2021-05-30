using ExampleService.Infrastructure.Entities;
using ExampleService.Infrastructure.Interface.IRepository.ExampleRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExampleService.Infrastructure.Interface.Repository.ExampleRepository
{
    public class ExampleRepository : Repository<BaseEntity>, IExampleRepository
    {
        private readonly ApplicationDbContext _context;
        public ExampleRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _context = dbContext;
        }
    }
}
