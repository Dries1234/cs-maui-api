using API.dtos;
using API.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net.Mail;

namespace API.Models
{

    public class RepoNode
    {
        public IEnumerable<Repository> nodes { get;set; }
    }

    public class Repository
    {
        public string id { get; set; } 
        public string name { get; set; }
        public string description { get; set; }
        public int stargazerCount { get; set; }
        public Language primaryLanguage { get; set; }

        public string url { get; set; }
    }
    public class Language
    {
        public int id { get; set; }

        public string name { get; set; }
    }
}

namespace API.dtos
{
    public class RepositoryWriteDto
    {

        public string id { get; set; } 
        public string name { get; set; }
        public string description { get; set; }
        public int stargazerCount { get; set; }
        public LanguageWriteDto primaryLanguage { get; set; }

        public string url { get; set; }
    }
    public class LanguageWriteDto
    {
        public string name { get; set; }
    }
}

namespace API.Profiles
{
    class RepositoryProfile : Profile
    {
        public RepositoryProfile()
        {
            CreateMap<RepositoryWriteDto, Repository>();
        }
    }
    class LanguageProfile: Profile
    {
        public LanguageProfile()
        {
            CreateMap<LanguageWriteDto, Language>();
        }
    }
}
