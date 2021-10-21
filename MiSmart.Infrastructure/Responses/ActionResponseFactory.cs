

using Microsoft.Extensions.Options;

namespace MiSmart.Infrastructure.Responses
{
    public interface IActionResponseFactory
    {
        ActionResponse CreateInstance();
        ActionResponseSettings Settings { get; }
    }
    public class ActionResponseFactory : IActionResponseFactory
    {
        private readonly ActionResponseSettings actionResponseSettings;
        public ActionResponseSettings Settings
        {
            get => actionResponseSettings;
        }
        public ActionResponseFactory(IOptions<ActionResponseSettings> options)
        {
            this.actionResponseSettings = options.Value;
        }

        public ActionResponse CreateInstance()
        {
            var response = new ActionResponse();
            response.ApplySettings(this.actionResponseSettings);
            return response;
        }


    }

}