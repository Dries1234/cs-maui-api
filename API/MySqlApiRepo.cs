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
        Boolean RemoveRepository(Repository repository);


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
            var existing = _context.Language.AsNoTracking().FirstOrDefault(l => l.name == repository.primaryLanguage.name);
            if (existing != null)
            {
                repository.primaryLanguage.id = existing.id; 
            }
            if(_context.Repositories.Any(r => r.id == repository.id))
            {
                Console.WriteLine("Repo already exists???");
                return false;
            }
            _context.Language.Attach(repository.primaryLanguage);
            _context.Repositories.Add(repository);
            save();
            Console.WriteLine("Returns true");
            return true;
        }

        public Boolean RemoveRepository(Repository repository)
        {
            var ToRemove = _context.Repositories.FirstOrDefault(r => r.id == repository.id);
            if (ToRemove == null) return false;
            else
            {
                _context.Repositories.Remove(repository);
                save();
                return true;
            }
        }

        private void save()
        {
            _context.SaveChanges();
        }
    }
}
