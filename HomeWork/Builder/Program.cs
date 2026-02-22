using System;

class Program
{
    static void Main()
    {
        Console.Write("Header: ");
        string header = Console.ReadLine();

        Console.Write("Content: ");
        string content = Console.ReadLine();

        Console.Write("Footer: ");
        string footer = Console.ReadLine();

        Director director = new Director();
        IReportBuilder builder = new SimpleReportBuilder();

        Report report = director.BuildReport(builder, header, content, footer);

        Console.WriteLine();
        report.Print();
    }
}