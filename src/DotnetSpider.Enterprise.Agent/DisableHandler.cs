namespace DotnetSpider.Enterprise.Agent
{
    internal class DisableHandler:BaseHandler
    {
        private Command command;

        public DisableHandler(Command command)
       : base(command)
        {
        }

        public override void Handle()
        {
            AgentConsts.IsEnabled = false;
        }
    }
}