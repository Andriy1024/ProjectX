using FluentEmail.Core;
using Microsoft.Extensions.Options;
using ProjectX.Core;
using System.Threading.Tasks;

namespace ProjectX.Email
{
    public sealed class EmailSender : IEmailSender
    {
        private readonly IFluentEmailFactory _emailFactory;
        private readonly IRazorViewToStringRenderer _razorRenderer;
        private readonly EmailOptions _options;
        public bool IsEmailSenderEnabled => _options.EnableEmailSender;

        public EmailSender(IFluentEmailFactory emailFactory,
            IRazorViewToStringRenderer razorRenderer,
            IOptions<EmailOptions> options)
        {
            _emailFactory = emailFactory;
            _razorRenderer = razorRenderer;
            _options = options.Value;
        }

        public async Task<Result> SendAsync(string to, string subject, string body, string senderName = null, bool isHtml = true)
        {
            if (!IsEmailSenderEnabled) 
            {
                return Error.InvalidData(ErrorCode.InvalidData, "The email sender is disabled.");
            }
                
            senderName ??= _options.FromName ?? _options.FromEmail; 

            var result = await _emailFactory
                                .Create()
                                .To(to)
                                .SetFrom(_options.FromEmail, senderName)
                                .Subject(subject)
                                .Body(body, isHtml)
                                .SendAsync();

            if (!result.Successful) 
            {
                return Error.ServerError($"EmailSender: {string.Join(", ", result.ErrorMessages)}");
            }

            return Result.Success;
        }

        public async Task<Result> SendAsync<TModel>(string to, string subject, string path, TModel model, string senderName)
        {
            if (!IsEmailSenderEnabled) 
            {
                return Error.InvalidData(ErrorCode.InvalidData, "The email sender is disabled.");
            }
                
            var body = await _razorRenderer.RenderViewToStringAsync(path, model);
            return await SendAsync(to, subject, body, senderName: senderName, isHtml: true);
        }
    }
}
