namespace API
{

    public class RepoNode
    {
        public IEnumerable<Repository> nodes { get;set; }
    }

    public class Repository
    {
        public string name { get; set; }
    }
}
