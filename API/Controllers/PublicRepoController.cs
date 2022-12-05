using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class PublicRepoController : ControllerBase
    {

        HttpClient client;

        IConfiguration configuration;
        public PublicRepoController()
        {
            configuration = new ConfigurationBuilder().AddJsonFile($"appsettings.json").Build();
            client = new HttpClient
            {
                BaseAddress = new Uri("https://api.github.com/graphql")
            };
            client.DefaultRequestHeaders.Add("User-Agent", "gitinder");
            client.DefaultRequestHeaders.Add("Authorization", $"bearer {configuration.GetValue<string>("GithubToken")}");

        }

        [HttpGet(Name = "Repos")]
        public IEnumerable<Repository> Get()
        {
            var res = GetPublicRepos();
            Console.WriteLine(res.Result["data"]["search"]);
            var result = JsonSerializer.Deserialize<RepoNode>(res.Result["data"]["search"]);
            return result.nodes;
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
