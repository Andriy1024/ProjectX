using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using System.Net;
using FluentEmail.Smtp;

namespace ProjectX.Email
{
    public static class EmailServiceCollectionExtensions
    {
        public static IServiceCollection AddEmailServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<EmailOptions>(configuration.GetSection(nameof(EmailOptions)));
            var options = configuration.GetSection("EmailOptions").Get<EmailOptions>();
            EmailOptions.Validate(options);

            if (!options.EnableEmailSender)
                return services.AddSingleton<IEmailSender, NoEmailSender>();

            var builder = services
                    .AddScoped<IEmailSender, EmailSender>()
                    .AddScoped<IRazorViewToStringRenderer, RazorViewToStringRenderer>()
                    .AddFluentEmail(defaultFromEmail: options.FromEmail, defaultFromName: options.FromName);

            if (options.SendGrid != null)
            {
                builder.AddSendGridSender(options.SendGrid.API_KEY);
            }
            else
            {
                services.AddSingleton<FluentEmail.Core.Interfaces.ISender>(x => new SmtpSender(new Func<SmtpClient>(() =>
                {
                    var client = new SmtpClient(options.SMTP.Host, options.SMTP.Port.Value)
                    {
                        EnableSsl = true,
                        Credentials = new NetworkCredential(options.FromEmail, options.SMTP.Password)
                    };
                    client.SendCompleted += (object sender, System.ComponentModel.AsyncCompletedEventArgs e) =>
                    {
                        if (e.Error != null)
                            System.Diagnostics.Trace.TraceError($"Error sending email: {e.Error.Message}");
                        if (sender is SmtpClient s)
                            s.Dispose();
                    };
                    return client;
                })));
            }

            return services;
        }
    }
}
