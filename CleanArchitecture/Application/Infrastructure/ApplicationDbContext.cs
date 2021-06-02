using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Infrastructure
{
    public interface IApplicationDbContext<T> where T : DbContext
    {
        void CreateDbContext();
    }
    class ApplicationDbContext : DbContext
    {
        public void CreateDbContext()
        {
            throw new NotImplementedException();
        }
    }
}
