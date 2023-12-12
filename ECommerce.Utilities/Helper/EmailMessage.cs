using MimeKit;

namespace ECommerce.Utilities.Helper
{
    public class Message
    {
        public List<MailboxAddress> ToEmailList { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }

        public Message(IEnumerable<string> to, string subject, string content)
        {
            ToEmailList = new List<MailboxAddress>();
            ToEmailList.AddRange(to.Select(x => new MailboxAddress("email", x)));
            Subject = subject;
            Content = content;
        }
    }
}
