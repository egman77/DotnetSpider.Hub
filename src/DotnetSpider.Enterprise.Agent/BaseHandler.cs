namespace DotnetSpider.Enterprise.Agent
{
    public abstract class BaseHandler
    {
        protected Command Command { get; set; }

        protected BaseHandler(Command command)
        {
            Command = command;
        }

        public abstract void Handle();
    }
}