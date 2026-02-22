using System;

public interface IReportBuilder
{
    void SetHeader(string header);
    void SetContent(string content);
    void SetFooter(string footer);
    Report GetReport();
}

public class SimpleReportBuilder : IReportBuilder
{
    private readonly Report report = new Report();

    public void SetHeader(string header) => report.Header = header;
    public void SetContent(string content) => report.Content = content;
    public void SetFooter(string footer) => report.Footer = footer;

    public Report GetReport() => report;
}