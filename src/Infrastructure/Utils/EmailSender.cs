using Domain.Interfaces.Utils;
using System.Net;
using System.Net.Mail;

namespace Infrastructure.Utils;

public class EmailSender : IEmailSender
{
    public async Task SendRecoverPasswordAsync(string email, string token)
    {
        var mail = Environment.GetEnvironmentVariable("EMAIL");
        var password = Environment.GetEnvironmentVariable("EMAIL_PASSWORD");
        var baseUrl = Environment.GetEnvironmentVariable("BASE_URL");

        var smtpClient = new SmtpClient("smtp.gmail.com", 587)
        {
            Credentials = new NetworkCredential(mail, password),
            EnableSsl = true,
            UseDefaultCredentials = false,
        };

        var url = $"{baseUrl}/reset-password?token={token}";
        try {
            await smtpClient.SendMailAsync(
                new MailMessage(
                    from: mail,
                    to: email,
                    subject: "Recuperación de Contraseña",
                    body: $"Hola,\n\nPara recuperar tu contraseña, por favor haz clic en el siguiente enlace:\n\n{url}\n\nSi no solicitaste la recuperación de la contraseña, ignora este mensaje."));

        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
}
