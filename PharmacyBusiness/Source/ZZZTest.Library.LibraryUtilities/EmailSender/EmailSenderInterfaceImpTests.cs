using Library.LibraryUtilities.EmailSender;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;
using System.Net.Mail;

namespace ZZZTest.Library.LibraryUtilities.EmailSender
{
    public class EmailSenderInterfaceImpTests
    {
        private readonly EmailSenderInterfaceImp _sut;
        private readonly ILogger<EmailSenderInterfaceImp> _mockLogger = Substitute.For<ILogger<EmailSenderInterfaceImp>>();

        public EmailSenderInterfaceImpTests()
        {
            _sut = new EmailSenderInterfaceImp(_mockLogger);
        }

        [Theory]
        [InlineData("Patrick.Starfish@BikiniBottom.com", "Sponge.Bob@BikiniBottom.com", "Let's go jelly fishing!", true)]
        [InlineData("", "Sponge.Bob@BikiniBottom.com", "Let's go jelly fishing!", false)]
        public void MessageHasOneSender_ShouldReturnExpectedResult_WhenAllConditionsAreMet(string from, string to, string subject, bool expectedValue)
        {
            // Setup
            MailMessage mailMessage;

            if (string.IsNullOrEmpty(from))
            {
                mailMessage = new MailMessage();
                //mailMessage.From = new MailAddress(from);
                mailMessage.To.Add(new MailAddress(to));
                mailMessage.Subject = subject;
            }
            else
            {
                mailMessage = new MailMessage
                {
                    From = new MailAddress(from),
                    To = { new MailAddress(to) },
                    Subject = subject
                };
            }

            // Act
            bool actualResult1 = _sut.MessageHasOneSender(mailMessage);
            bool actualResult2 = _sut.IsMailMessageValid(mailMessage);

            // Assert
            Assert.Equal(expectedValue, actualResult1);
            Assert.Equal(expectedValue, actualResult2);
        }

        [Theory]
        [InlineData("Patrick.Starfish@BikiniBottom.com", "Sponge.Bob@BikiniBottom.com", "Let's go jelly fishing!", true)]
        [InlineData("Patrick.Starfish@BikiniBottom.com", "", "Let's go jelly fishing!", false)]
        public void MessageHasAtLeastOneRecipient_ShouldReturnExpectedResult_WhenAllConditionsAreMet(string from, string to, string subject, bool expectedValue)
        {
            // Setup
            MailMessage mailMessage;

            if (string.IsNullOrEmpty(to))
            {
                mailMessage = new MailMessage();
                mailMessage.From = new MailAddress(from);
                //mailMessage.To.Add(new MailAddress(to));
                mailMessage.Subject = subject;
            }
            else
            {
                mailMessage = new MailMessage
                {
                    From = new MailAddress(from),
                    To = { new MailAddress(to) },
                    Subject = subject
                };
            }

            // Act
            bool actualResult1 = _sut.MessageHasAtLeastOneRecipient(mailMessage);
            bool actualResult2 = _sut.IsMailMessageValid(mailMessage);

            // Assert
            Assert.Equal(expectedValue, actualResult1);
            Assert.Equal(expectedValue, actualResult2);
        }

        [Theory]
        [InlineData("Patrick.Starfish@BikiniBottom.com", "Sponge.Bob@BikiniBottom.com", "Let's go jelly fishing!", true)]
        [InlineData("Patrick.Starfish@BikiniBottom.com", "Sponge.Bob@BikiniBottom.com", "", false)]
        public void MessageHasSubjectLine_ShouldReturnExpectedResult_WhenAllConditionsAreMet(string from, string to, string subject, bool expectedValue)
        {
            // Setup
            MailMessage mailMessage;

            if (string.IsNullOrEmpty(subject))
            {
                mailMessage = new MailMessage();
                mailMessage.From = new MailAddress(from);
                mailMessage.To.Add(new MailAddress(to));
                //mailMessage.Subject = subject;
            }
            else
            {
                mailMessage = new MailMessage
                {
                    From = new MailAddress(from),
                    To = { new MailAddress(to) },
                    Subject = subject
                };
            }

            // Act
            bool actualResult1 = _sut.MessageHasSubjectLine(mailMessage);
            bool actualResult2 = _sut.IsMailMessageValid(mailMessage);

            // Assert
            Assert.Equal(expectedValue, actualResult1);
            Assert.Equal(expectedValue, actualResult2);
        }
    }
}
