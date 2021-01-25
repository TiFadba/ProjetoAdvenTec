using ProjetoAdvenTec.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ProjetoAdvenTec.Services
{
    /*Validação de formato de email e envio de email com link da avaliação*/

    public class EmailService
    {
        //Enviar emails
        public static int EnviarEmailAutomatico(EmailModel model)
        {

            /*Configurações para o gmail*/

            //Adicione um gmail válido
            MailMessage mm = new MailMessage("seu@gmail.com", model.to);
            mm.Subject = model.subject;
            mm.Body = model.body;
            mm.IsBodyHtml = true;

            SmtpClient smtp = new SmtpClient();
            smtp.Host = "smtp.gmail.com";
            smtp.Port = 587;
            smtp.EnableSsl = true;

            //Adicione o email(o mesmo que o anterior) e as credencias de acesso(senha)
            NetworkCredential nc = new NetworkCredential("seu@gmail.com", "sua senha");
            smtp.UseDefaultCredentials = true;
            smtp.Credentials = nc;
            try
            {
                smtp.Send(mm);
                return 1;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return 0;
            }

        }

        public static bool ValidarFormatoEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                // Normalize the domain
                email = Regex.Replace(email, @"(@)(.+)$", DomainMapper,
                                      RegexOptions.None, TimeSpan.FromMilliseconds(200));

                // Examines the domain part of the email and normalizes it.
                string DomainMapper(Match match)
                {
                    // Use IdnMapping class to convert Unicode domain names.
                    var idn = new IdnMapping();

                    // Pull out and process domain name (throws ArgumentException on invalid)
                    string domainName = idn.GetAscii(match.Groups[2].Value);

                    return match.Groups[1].Value + domainName;
                }
            }
            catch (RegexMatchTimeoutException e)
            {
                return false;
            }
            catch (ArgumentException e)
            {
                return false;
            }

            try
            {
                return Regex.IsMatch(email,
                    @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                    RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }
    }
}
