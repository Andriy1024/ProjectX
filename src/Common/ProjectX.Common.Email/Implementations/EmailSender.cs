using FluentEmail.Core;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace ProjectX.Common.Email
{
    public sealed class EmailSender : IEmailSender
    {
        readonly IFluentEmailFactory _emailFactory;
        readonly IRazorViewToStringRenderer _razorRenderer;
        readonly EmailOptions _options;

        public EmailSender(IFluentEmailFactory emailFactory,
            IRazorViewToStringRenderer razorRenderer,
            IOptions<EmailOptions> options)
        {
            _emailFactory = emailFactory;
            _razorRenderer = razorRenderer;
            _options = options.Value;
        }

        public async Task SendAsync(string to, string subject, string body, string senderName = null, bool isHtml = true)
        {
            if (!_options.EnableEmailSender)
                return;

            senderName ??= _options.FromName ?? _options.FromEmail; 

            var result = await _emailFactory
                                .Create()
                                .To(to)
                                .SetFrom(_options.FromEmail, senderName)
                                .Subject(subject)
                                .Body(body, isHtml)
                                .SendAsync();

            if (!result.Successful)
                 throw new Exception($"EmailSender: {string.Join(", ", result.ErrorMessages)}");
        }

        public async Task SendAsync<TModel>(string to, string subject, string path, TModel model, string senderName)
        {
            if (!_options.EnableEmailSender)
                return;

            var body = await _razorRenderer.RenderViewToStringAsync(path, model);
            await SendAsync(to, subject, body, senderName: senderName, isHtml: true);
        }
    }
}
