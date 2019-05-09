using MimeKit;
using System.Threading.Tasks;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;
namespace ChuvsuEvents.Models {
    public class EmailService {
        public async Task SendEmailAsync(string email, string subject, string message) {
            var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress("Администрация сайта", "chuvsuevents@u0680543.plsk.regruhosting.ru"));
            emailMessage.To.Add(new MailboxAddress("", email));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html) {
                Text = message
            };

            using (var client = new SmtpClient()) {
                //client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                //await client.ConnectAsync("smtp.yandex.ru", 25, false);
                await client.ConnectAsync("mail.u0680543.plsk.regruhosting.ru", 25, false);
                await client.AuthenticateAsync("chuvsuevents@u0680543.plsk.regruhosting.ru", "gromon0331");
                await client.SendAsync(emailMessage);

                await client.DisconnectAsync(true);

            }
        }
    }
}
