namespace DotnetSpider.Enterprise.Agent
{
    internal class EnableHandler:BaseHandler
    {
        private Command command;

        public EnableHandler(Command command) : base(command)
        {
        }

        public override void Handle()
        {
            AgentConsts.IsEnabled = true;
        }
    }
}