using System.Collections.Generic;
using System.Linq;
using API.dtos;
using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Services
{
    public interface IApiRepo
    {
        IEnumerable<Repository> GetRepositories();
        Repository GetRepository(string id);

        Boolean AddRepository(Repository repository);


    }
     class MySqlApiRepo : IApiRepo
    {
        private readonly ApiContext _context;
        public MySqlApiRepo(ApiContext context)
        {
            _context = context;
        }

        public IEnumerable<Repository> GetRepositories()
        {
            return _context.Repositories.ToList();
        }

        public Repository GetRepository(string id)
        {
            return _context.Repositories.FirstOrDefault(r => r.id == id);
        }

        public Boolean AddRepository(Repository repository)
        {
            if(_context.Repositories.Any(r => r.id == repository.id))
            {
                return false;
            }

            _context.Repositories.Add(repository);
            save();
            return true;
        }

        private void save()
        {
            _context.SaveChanges();
        }
    }
}
