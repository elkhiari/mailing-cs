using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;

class Person
{
    private Guid uid = Guid.NewGuid();
    private string name;
    private int age;
    private string email;

    public Person(string name, int age, string email)
    {
        Name = name;
        Age = age;
        Email = email;
    }

    public string Name
    {
        get { return name; }
        set { name = value; }
    }

    public int Age
    {
        get { return age; }
        set
        {
            if (value < 0)
            {
                throw new ArgumentException("Age must be a positive number");
            }
            age = value;
        }
    }

    public string Email
    {
        get { return email; }
        set
        {
            EmailAddressAttribute emailAttribute = new EmailAddressAttribute();
            if (!emailAttribute.IsValid(value))
            {
                throw new ArgumentException("Invalid email address");
            }
            email = value;
        }
    }

    public void Write()
    {
        Console.WriteLine("Name: " + Name);
        Console.WriteLine("Age: " + Age);
        Console.WriteLine("Email: " + Email);
    }
}

class Mail
{
    public static void Send(string to, string subject, string body)
    {
        try
        {
            Console.WriteLine("Sending email to " + to);
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress("elkhiariothmane@gmail.com");
            mail.To.Add(to);
            mail.Subject = subject;
            mail.Body = body;

            SmtpClient smtp = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("elkhiariothmane@gmail.com", "cc"),
                EnableSsl = true
            };

            smtp.Send(mail);
            Console.WriteLine("Email sent successfully");
        }
        catch (Exception e)
        {
            Console.WriteLine("Error: " + e.Message);
        }
    }

    public static async Task SendMailAsync(string to, string subject, string body)
    {
        try
        {
            Console.WriteLine("Sending email to " + to);
            MailMessage mail = new MailMessage
            {
                From = new MailAddress("elkhiariothmane@gmail.com"),
                Subject = subject,
                Body = body
            };
            mail.To.Add(to);

            SmtpClient smtp = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("elkhiariothmane@gmail.com", "jyyjtsgsyilbdefw"),
                EnableSsl = true
            };

            using (var msg = mail)
            {
                await smtp.SendMailAsync(msg);
            }

            Console.WriteLine("Email sent successfully");
        }
        catch (Exception e)
        {
            Console.WriteLine("Error: " + e.Message);
        }
    }

    public static void SendMailConcurrently(List<string> to, string subject, string body)
    {
        var tasks = new List<Task>();
        foreach (var email in to)
        {
            tasks.Add(Task.Run(() => Send(email, subject, body)));
        }
        Task.WaitAll(tasks.ToArray());
    }

    public static async Task SendMailConcurrentlyForLargeAsync(List<string> to, string subject, string body, int maxDegreeOfParallelism)
    {
        var semaphore = new SemaphoreSlim(maxDegreeOfParallelism);
        var tasks = new List<Task>();

        foreach (var email in to)
        {
            await semaphore.WaitAsync();

            var task = SendMailAsync(email, subject, body).ContinueWith(t => semaphore.Release());
            tasks.Add(task);
        }

        await Task.WhenAll(tasks);
    }
}

class Program
{
    static async Task Main()
    {
        List<string> mails = new List<string>
        {
            "nizarbouali@gmail.com",
            "nizarbouali@gmail.com","nizarbouali@gmail.com","nizarbouali@gmail.com","nizarbouali@gmail.com","nizarbouali@gmail.com","nizarbouali@gmail.com","nizarbouali@gmail.com","nizarbouali@gmail.com","nizarbouali@gmail.com","nizarbouali@gmail.com","nizarbouali@gmail.com","nizarbouali@gmail.com","nizarbouali@gmail.com","nizarbouali@gmail.com","nizarbouali@gmail.com","nizarbouali@gmail.com","nizarbouali@gmail.com","nizarbouali@gmail.com","nizarbouali@gmail.com","nizarbouali@gmail.com","nizarbouali@gmail.com","nizarbouali@gmail.com","nizarbouali@gmail.com","nizarbouali@gmail.com","nizarbouali@gmail.com","nizarbouali@gmail.com","nizarbouali@gmail.com","nizarbouali@gmail.com","nizarbouali@gmail.com","nizarbouali@gmail.com","nizarbouali@gmail.com","nizarbouali@gmail.com","nizarbouali@gmail.com","nizarbouali@gmail.com",
        };

        await Mail.SendMailConcurrentlyForLargeAsync(mails, "Test", "Test", 10);
    }
}
