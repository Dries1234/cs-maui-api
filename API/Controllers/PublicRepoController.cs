using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using API.Models;
using API.Services;
using AutoMapper;
using API.dtos;

namespace API.Controllers
{
    [ApiController]
    public class PublicRepoController : ControllerBase
    {

        HttpClient client;

        IConfiguration configuration;
        private readonly IApiRepo _repo;
        private readonly IMapper _map;
        public PublicRepoController(IApiRepo repo, IMapper map)
        {
            _repo = repo;
            _map = map;
            configuration = new ConfigurationBuilder().AddJsonFile($"appsettings.json").Build();
            client = new HttpClient
            {
                BaseAddress = new Uri("https://api.github.com/graphql")
            };
            client.DefaultRequestHeaders.Add("User-Agent", "gitinder");
            client.DefaultRequestHeaders.Add("Authorization", $"bearer {configuration.GetValue<string>("GithubToken")}");

        }

        [HttpGet]
        [Route("publicrepos")]
        public IEnumerable<Repository> Get()
        {
            var res = GetPublicRepos();
            Console.WriteLine(res.Result["data"]["search"]);
            var result = JsonSerializer.Deserialize<RepoNode>(res.Result["data"]["search"]);
            return result.nodes;
        }

        [HttpGet]
        [Route("matches")]
        public ActionResult GetMatches()
        {
            return Ok(_map.Map<Repository>(_repo.GetRepositories())); 
        }

        [HttpPost]
        [Route("matches/add")]
        public ActionResult AddMatch(RepositoryWriteDto repositoryWriteDto)
        {
            var repo = _map.Map<Repository>(repositoryWriteDto);
            if (_repo.AddRepository(repo)) { 
                return Ok();
            }
            else
            {
                return ValidationProblem();
            }
        }

        private async Task<JsonNode> GetPublicRepos()
        {
            var queryObject = new
            {
                query = @"{
                        search(query: ""is:public stars:>100 pushed:>=2022-08-20"", type: REPOSITORY, first: 50) {
                                nodes {
                                    ... on Repository {
                                         id,
                                         name
                                         description
                                         stargazerCount
                                         primaryLanguage {
                                           name
                                         },
                                         url
                                    }
                                }
                        }
                }",
                variables = new { }
            };
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                Content = new StringContent(JsonSerializer.Serialize(queryObject), Encoding.UTF8, "application/json")
            };
           JsonNode responseObj;

            using var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();
            responseObj = JsonNode.Parse(responseString);
            return responseObj;
        }

    }
}
