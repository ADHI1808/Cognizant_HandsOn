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