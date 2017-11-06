namespace DotnetSpider.Enterprise.Agent
{
    public class Command
    {
        public const string Run = "RUN";
        public const string Publish = "PUBLISH";
        public const string DeleteProject = "DELETE_PROJECT";
        public const string DeleteTask = "DELETE_TASK";
        public const string Enable = "ENABLE";
        public const string Disable = "DISABLE";

        public string Id { get; set; }
        public string Target { get; set; }
        public string Name { get; set; }
        public string Data { get; set; }
    }
}