using ProjectX.Core;
using System.Threading.Tasks;

namespace ProjectX.Email
{
    public sealed class NoEmailSender : IEmailSender
    {
        public bool IsEmailSenderEnabled => false;

        public Task<Result> SendAsync(string to, string subject, string body, string senderName = null, bool isHtml = true)
        {
            return Task.FromResult(Result.Success);
        }

        public Task<Result> SendAsync<TModel>(string to, string subject, string path, TModel model, string senderName = null)
        {
            return Task.FromResult(Result.Success);
        }
    }
}
