namespace PwC.CarRental.EmailFunction.Configurations;

public class MailConfiguration
{
    public string Host { get; set; }
    public int Port { get; set; }
    public string User { get; set; }
    public string Password { get; set; }
    public string From { get; set; }
    public string FromName { get; set; }
}
