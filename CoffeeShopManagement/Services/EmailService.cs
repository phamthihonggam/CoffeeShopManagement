using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace CoffeeShopManagement.Services
{
    public class EmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendOtpAsync(
            string toEmail,
            string otp)
        {
            var senderEmail =
                _configuration["EmailSettings:SenderEmail"];

            var senderName =
                _configuration["EmailSettings:SenderName"];

            var appPassword =
                _configuration["EmailSettings:AppPassword"];

            if (string.IsNullOrWhiteSpace(senderEmail) ||
                string.IsNullOrWhiteSpace(appPassword))
            {
                throw new Exception(
                    "Chưa cấu hình EmailSettings trong User Secrets.");
            }

            var message = new MimeMessage();

            message.From.Add(
                new MailboxAddress(
                    senderName ?? "Rosalie Coffee",
                    senderEmail
                )
            );

            message.To.Add(
                MailboxAddress.Parse(toEmail)
            );

            message.Subject =
                "Mã xác nhận đặt lại mật khẩu - Rosalie Coffee";

            message.Body = new TextPart("html")
            {
                Text = $@"
                    <div style='font-family:Arial,sans-serif;
                                max-width:600px;
                                margin:auto;
                                padding:30px;
                                border:1px solid #eee;
                                border-radius:16px;'>

                        <h2 style='color:#6b3d2e;'>
                            Rosalie Coffee
                        </h2>

                        <p>
                            Bạn vừa yêu cầu đặt lại mật khẩu.
                        </p>

                        <p>
                            Mã xác nhận của bạn là:
                        </p>

                        <div style='
                            font-size:32px;
                            font-weight:bold;
                            letter-spacing:8px;
                            color:#6b3d2e;
                            margin:25px 0;'>

                            {otp}

                        </div>

                        <p>
                            Mã này có hiệu lực trong
                            <strong>5 phút</strong>.
                        </p>

                        <p style='color:#777;'>
                            Nếu bạn không yêu cầu đổi mật khẩu,
                            hãy bỏ qua email này.
                        </p>

                    </div>"
            };

            using var client = new SmtpClient();

            await client.ConnectAsync(
                "smtp.gmail.com",
                587,
                SecureSocketOptions.StartTls
            );

            await client.AuthenticateAsync(
                senderEmail,
                appPassword
            );

            await client.SendAsync(message);

            await client.DisconnectAsync(true);
        }
    }
}