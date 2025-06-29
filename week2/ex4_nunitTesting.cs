/*Step1-Create folder in vs code and execute below in terminal
dotnet new sln -n CustomerApp
dotnet new classlib -n CustomerCommLib
dotnet new nunit -n CustomerComm.Tests
dotnet sln CustomerApp.sln add CustomerCommLib/CustomerCommLib.csproj
dotnet sln CustomerApp.sln add CustomerComm.Tests/CustomerComm.Tests.csproj
cd CustomerComm.Tests
dotnet add reference ../CustomerCommLib/CustomerCommLib.csproj
cd CustomerComm.Tests
dotnet add package Moq
dotnet add package NUnit
dotnet add package NUnit3TestAdapter
dotnet add package Microsoft.NET.Test.Sdk*/

//step2 - create ImailSender inside CustomerCommLib
namespace CustomerCommLib
{
    public interface IMailSender
    {
        bool SendMail(string toAddress, string message);
    }
}

//step3- create CustomerComm inside CustomerCommLib
namespace CustomerCommLib
{
    public class CustomerComm
    {
        IMailSender _mailSender;
        public CustomerComm(IMailSender mailSender)
        {
            _mailSender = mailSender;
        }
        public bool SendMailToCustomer()
        {
            return _mailSender.SendMail("Adhi@abc.com", "hi hi");
        }
    }
}

//step4- create MailSender inside CustomerCommLib
using System.Net;
using System.Net.Mail;

namespace CustomerCommLib
{
    public class MailSender : IMailSender
    {
        public bool SendMail(string toAddress, string message)
        {
            MailMessage mail = new MailMessage();
            SmtpClient smtpServer = new SmtpClient("smtp.gmail.com");
            mail.From = new MailAddress("your_email_address@gmail.com");
            mail.To.Add(toAddress);
            mail.Subject = "Test Mail";
            mail.Body = message;
            smtpServer.Port = 587;
            smtpServer.Credentials = new NetworkCredential("username", "password");
            smtpServer.EnableSsl = true;
            smtpServer.Send(mail);
            return true;
        }
    }
}

//step5- create CustomerCommTests inside CustomercommTests
using NUnit.Framework;
using Moq;
using CustomerCommLib;

namespace CustomerCommTests
{
    [TestFixture]
    public class CustomerCommTests
    {
        private Mock<IMailSender> mockMailSender;
        private CustomerCommLib.CustomerComm customerComm;
        [OneTimeSetUp]
        public void Setup()
        {
            mockMailSender = new Mock<IMailSender>();
            mockMailSender
                .Setup(m => m.SendMail(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(true);

            customerComm = new CustomerCommLib.CustomerComm(mockMailSender.Object);
        }
        [Test]
        [TestCase("cust123@abc.com", "Some Message")]
        public void SendMailToCustomer_ShouldReturnTrue(string email, string message)
        {
            bool result = customerComm.SendMailToCustomer();
            Assert.That(result, Is.True);
        }
    }
}

//step6- type "dotnet test" in terminal for output


