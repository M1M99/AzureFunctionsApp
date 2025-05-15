using System.Net;
using System.Net.Mail;
using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AzureFunctionsApp
{
    public class Function1
    {
        private readonly ILogger _logger;

        public Function1(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<Function1>();
        }

        [Function("Function1")]
        public HttpResponseData Run([HttpTrigger(AuthorizationLevel.Admin, "get", "post","put")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            var name = req.Query["name"];
            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

            response.WriteString($"Welcome {name} to Azure Functions!");

            return response;
        }     
        [Function("Function4")]
        public async Task<HttpResponseData> Run4([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req)
        {
            var response = req.CreateResponse();

            try
            {
                using var reader = new StreamReader(req.Body);
                var requestBody = await reader.ReadToEndAsync();
                var data = JsonSerializer.Deserialize<Dictionary<string, string>>(requestBody);

                string name = data.GetValueOrDefault("name");
                string email = data.GetValueOrDefault("email");

                string smtpServer = "smtp.gmail.com";
                int smtpPort = 587;
                string senderEmail = "mahammad@gmail.com";
                string senderPassword = "asfdudhfg7rgi9jp";

                var mail = new MailMessage(senderEmail, email)
                {
                    Subject = $"Hi, {name}",
                    Body = $"Salam {name}, Written Azure Company"
                };

                using var smtpClient = new SmtpClient(smtpServer, smtpPort)
                {
                    Credentials = new NetworkCredential(senderEmail, senderPassword),
                    EnableSsl = true
                };

                await smtpClient.SendMailAsync(mail);

                response.StatusCode = HttpStatusCode.OK;
                await response.WriteStringAsync("Email sent successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Email failed: {ex.Message}");
                response.StatusCode = HttpStatusCode.InternalServerError;
                await response.WriteStringAsync($"Failed to send email: {ex.Message}");
            }

            return response;
        }

    }
}
