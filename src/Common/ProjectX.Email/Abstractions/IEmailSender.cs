using ProjectX.Core;
using System.Threading.Tasks;

namespace ProjectX.Email
{
    public interface IEmailSender
    {
        bool IsEmailSenderEnabled { get; }

        Task<Result> SendAsync(string to, string subject, string body, string senderName = null, bool isHtml = true);
        
        Task<Result> SendAsync<TModel>(string to, string subject, string path, TModel model, string senderName = null);
    }
}
