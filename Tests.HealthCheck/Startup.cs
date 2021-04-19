using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using HealthChecks.UI.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tests.HealthCheck.Models;
using Tests.HealthCheck.Models.Configurations;

namespace Tests.HealthCheck
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        private readonly HealthCheckEndpointsConfig _healthCheckConfig;
        private readonly SmtpCredentialsOptions _smtpCredentialsOptions;
        private static List<(string ServiceName, string Uri)> _servicesInfo;

        private string GetTokenFromAuth() 
        {
            AuthLoginConfig authLoginConfig = Configuration
                .GetSection(AuthLoginConfig.SectionName)
                .Get<AuthLoginConfig>();

            HttpWebRequest httpRequest = (HttpWebRequest) WebRequest
                .Create(authLoginConfig.UriString);

            string login = "admin";
            string password = "%4fgT1_3ioR";
            string stringData = $"{{ \"LoginData\": \"{login}\",\"Password\": \"{password}\" }}";

            byte[] data = Encoding.Default.GetBytes(stringData);

            httpRequest.Method = "POST";
            httpRequest.ContentType = "application/json; charset=utf-8";
            httpRequest.ContentLength = data.Length;

            using Stream newStream = httpRequest.GetRequestStream();
            newStream.Write(data, 0, data.Length);

            using HttpWebResponse httpResponse = (HttpWebResponse) httpRequest.GetResponse();
            using Stream stream = httpResponse.GetResponseStream();
            using StreamReader reader = new StreamReader(stream);

            string response = reader.ReadToEnd();
            var token = response.Split("\"token\":")[^1].Trim('}').Trim('\"');

            return token;
        }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            _healthCheckConfig = Configuration
                .GetSection(HealthCheckEndpointsConfig.SectionName)
                .Get<HealthCheckEndpointsConfig>();

            _smtpCredentialsOptions = Configuration
                .GetSection(SmtpCredentialsOptions.SectionName)
                .Get<SmtpCredentialsOptions>();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            string token = GetTokenFromAuth();

            services.AddControllers();

            services
                .AddHealthChecksUI(setupSettings: setup =>
                {
                    setup
                        .AddWebhookNotification("", uri: "",
                            payload: "{ message: \"Webhook report for [[LIVENESS]]: [[FAILURE]] - Description: [[DESCRIPTIONS]]\"}",
                            restorePayload: "{ message: \"[[LIVENESS]] is back to life\"}",
                            customDescriptionFunc: report =>
                            {
                                var failing = report.Entries
                                    .Where(e => e.Value.Status != UIHealthStatus.Healthy);

                                string[] emailSettings = Configuration
                                    .GetSection("SendEmailSettings")
                                    .Get<string[]>();

                                EmailSender.SendEmail(emailSettings, _smtpCredentialsOptions, report);

                                return $"{failing.Count()} healthchecks are failing";
                            });

                    string evaluationTimeString = Environment.GetEnvironmentVariable("EvaluationTimeInSeconds");
                    if (!string.IsNullOrEmpty(evaluationTimeString)
                        && int.TryParse(evaluationTimeString, out int evaluationTimeSeconds))
                    {
                        setup.SetEvaluationTimeInSeconds(evaluationTimeSeconds);
                    }

                    setup.ConfigureApiEndpointHttpclient((sp, client) =>
                    {
                        client.DefaultRequestHeaders.Add("token", token);
                    });

                    _servicesInfo = _healthCheckConfig
                        .GetType()
                        .GetProperties()
                        .Select(x => (ServiceName: x.Name, Uri: (string) x.GetValue(_healthCheckConfig)))
                        .ToList();

                    foreach (var (serviceName, uri) in _servicesInfo)
                    {
                        setup.AddHealthCheckEndpoint(
                            serviceName,
                            uri);
                    }
                })
                .AddInMemoryStorage();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
#if RELEASE
            app.UseHttpsRedirection();
#endif

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecksUI();
            });
        }
    }
}