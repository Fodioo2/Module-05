using System;

public class Report
{
    public string Header;
    public string Content;
    public string Footer;

    public void Print()
    {
        Console.WriteLine("===== REPORT =====");
        Console.WriteLine(Header);
        Console.WriteLine();
        Console.WriteLine(Content);
        Console.WriteLine();
        Console.WriteLine(Footer);
        Console.WriteLine("==================");
    }
}
