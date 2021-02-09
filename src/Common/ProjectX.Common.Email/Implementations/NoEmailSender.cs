using System.Threading.Tasks;

namespace ProjectX.Common.Email
{
    public sealed class NoEmailSender : IEmailSender
    {
        public Task SendAsync(string to, string subject, string body, string senderName = null, bool isHtml = true)
        {
            return Task.CompletedTask;
        }

        public Task SendAsync<TModel>(string to, string subject, string path, TModel model, string senderName = null)
        {
            return Task.CompletedTask;
        }
    }
}
