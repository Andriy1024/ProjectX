using System.Threading.Tasks;

namespace ProjectX.Email
{
    public interface IEmailSender
    {
        Task SendAsync(string to, string subject, string body, string senderName = null, bool isHtml = true);
        Task SendAsync<TModel>(string to, string subject, string path, TModel model, string senderName = null);
    }
}
