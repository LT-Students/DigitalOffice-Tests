using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using HealthChecks.UI.Configuration;
using HealthChecks.UI.Core;
using LT.DigitalOffice.Kernel.Configurations;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Kernel.Middlewares.ApiInformation;
using LT.DigitalOffice.Models.Broker.Requests.Company;
using LT.DigitalOffice.Tests.Models.Dto.Configurations;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tests.HealthCheck.Models.Configurations;
using Tests.HealthCheck.Models.Helpers;

namespace Tests.HealthCheck
{
    public class Startup : BaseApiInfo
    {
        public IConfiguration Configuration { get; }

        private readonly HealthCheckEndpointsConfig _healthCheckConfig;
        private static List<(string ServiceName, string Uri)> _servicesInfo;
        private static string[] _emails = new string[10];
        private static int _interval;
        private readonly RabbitMqConfig _rabbitMqConfig;
        private readonly BaseServiceInfoConfig _serviceInfoConfig;

        private void ConfigureHcEndpoints(Settings setup)
        {
            foreach (var (serviceName, uri) in _servicesInfo)
            {
                setup.AddHealthCheckEndpoint(
                    serviceName,
                    uri);
            }
        }

        private string GetTokenFromAuthService()
        {
            AuthLoginConfig authLoginConfig = Configuration
                .GetSection(AuthLoginConfig.SectionName)
                .Get<AuthLoginConfig>();

            HttpWebRequest httpRequest = (HttpWebRequest) WebRequest
                .Create(authLoginConfig.UriString);

            string stringData =
                $"{{ \"LoginData\": \"{authLoginConfig.Login}\",\"Password\": \"{authLoginConfig.Password}\" }}";

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

            /*_emails = Configuration
                .GetSection("SendEmailList")
                .Get<string[]>();*/

            _rabbitMqConfig = Configuration
                .GetSection(BaseRabbitMqConfig.SectionName)
                .Get<RabbitMqConfig>();

            if (!int.TryParse(Environment.GetEnvironmentVariable("SendIntervalInMinutes"), out var _interval))
            {
                _interval = Configuration.GetSection("SendIntervalInMinutes").Get<int>();
            }
        }

        public void ConfigureServices(IServiceCollection services)
        {
            string token = GetTokenFromAuthService();

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

                                ReportEmailSender.AddReport(report);

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

                    ConfigureHcEndpoints(setup);
                })
                .AddInMemoryStorage();

            services.AddMassTransit(x =>
            {
                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host("localhost", "/", host =>
                    {
                        host.Username("TestService_971E75B1-E475-4A2D-97A4-9A7FDE1FK8R9");
                        host.Password("971E75B1-E475-4A2D-97A4-9A7FDE1FK8R9");
                    });
                });

                x.AddRequestClients(_rabbitMqConfig);
            });

            services.AddMassTransitHostedService();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecksUI();
            });

            IServiceProvider serviceProvider = app.ApplicationServices.GetRequiredService<IServiceProvider>();

            var scope = app.ApplicationServices.CreateScope();

            IRequestClient<IGetSmtpCredentialsRequest> rcGetSmtpCredentials = serviceProvider.CreateRequestClient<IGetSmtpCredentialsRequest>(
                new Uri($"{_rabbitMqConfig.BaseUrl}/{_rabbitMqConfig.GetSmtpCredentialsEndpoint}"), default);

            SmtpGetter smtpGetter = new SmtpGetter(rcGetSmtpCredentials);

            if (smtpGetter.GetSmtp().Result)
            {
                Task.Run(() => ReportEmailSender.Start(_interval, _emails));
            }
        }
    }
}