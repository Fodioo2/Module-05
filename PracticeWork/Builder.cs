using System;
using System.Collections.Generic;
using System.IO;

public class ReportStyle
{
    public string BackgroundColor { get; set; } = "white";
    public string FontColor { get; set; } = "black";
    public int FontSize { get; set; } = 14;
    public string FontFamily { get; set; } = "Arial";
}

public class Report
{
    public string Header { get; set; } = "";
    public string Content { get; set; } = "";
    public string Footer { get; set; } = "";
    public List<(string Name, string Content)> Sections { get; set; } = new();
    public ReportStyle Style { get; set; } = new ReportStyle();

    public string Rendered { get; set; } = "";
    public string Format { get; set; } = "TEXT"; 

    public void Export(string path)
    {
        File.WriteAllText(path, Rendered);
    }
}

public interface IReportBuilder
{
    void SetHeader(string header);
    void SetContent(string content);
    void SetFooter(string footer);
    void AddSection(string sectionName, string sectionContent);
    void SetStyle(ReportStyle style);
    Report GetReport();
}

public class TextReportBuilder : IReportBuilder
{
    private readonly Report report = new Report();

    public void SetHeader(string header) => report.Header = header;
    public void SetContent(string content) => report.Content = content;
    public void SetFooter(string footer) => report.Footer = footer;
    public void AddSection(string sectionName, string sectionContent)
        => report.Sections.Add((sectionName, sectionContent));
    public void SetStyle(ReportStyle style) => report.Style = style;

    public Report GetReport()
    {
        report.Format = "TEXT";

        var lines = new List<string>();
        lines.Add("===== REPORT =====");
        lines.Add(report.Header);
        lines.Add("");

        lines.Add(report.Content);
        lines.Add("");

        foreach (var s in report.Sections)
        {
            lines.Add($"--- {s.Name} ---");
            lines.Add(s.Content);
            lines.Add("");
        }

        lines.Add(report.Footer);
        lines.Add("==================");

        report.Rendered = string.Join(Environment.NewLine, lines);
        return report;
    }
}

public class HtmlReportBuilder : IReportBuilder
{
    private readonly Report report = new Report();

    public void SetHeader(string header) => report.Header = header;
    public void SetContent(string content) => report.Content = content;
    public void SetFooter(string footer) => report.Footer = footer;
    public void AddSection(string sectionName, string sectionContent)
        => report.Sections.Add((sectionName, sectionContent));
    public void SetStyle(ReportStyle style) => report.Style = style;

    public Report GetReport()
    {
        report.Format = "HTML";

        string style =
            $"background:{report.Style.BackgroundColor};" +
            $"color:{report.Style.FontColor};" +
            $"font-size:{report.Style.FontSize}px;" +
            $"font-family:{report.Style.FontFamily};";

        var html = new List<string>();
        html.Add("<!doctype html>");
        html.Add("<html><head><meta charset='utf-8'></head>");
        html.Add($"<body style='{style}'>");
        html.Add($"<h1>{Escape(report.Header)}</h1>");
        html.Add($"<p>{Escape(report.Content)}</p>");

        foreach (var s in report.Sections)
        {
            html.Add($"<h2>{Escape(s.Name)}</h2>");
            html.Add($"<p>{Escape(s.Content)}</p>");
        }

        html.Add($"<footer><small>{Escape(report.Footer)}</small></footer>");
        html.Add("</body></html>");

        report.Rendered = string.Join(Environment.NewLine, html);
        return report;
    }

    private static string Escape(string s)
        => s.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;");
}

public class PdfReportBuilder : IReportBuilder
{
    private readonly Report report = new Report();

    public void SetHeader(string header) => report.Header = header;
    public void SetContent(string content) => report.Content = content;
    public void SetFooter(string footer) => report.Footer = footer;
    public void AddSection(string sectionName, string sectionContent)
        => report.Sections.Add((sectionName, sectionContent));
    public void SetStyle(ReportStyle style) => report.Style = style;

    public Report GetReport()
    {
        report.Format = "PDF";

        var lines = new List<string>();
        lines.Add("[PDF REPORT]");
        lines.Add(report.Header);
        lines.Add(report.Content);

        foreach (var s in report.Sections)
        {
            lines.Add($"## {s.Name}");
            lines.Add(s.Content);
        }

        lines.Add(report.Footer);

        report.Rendered = string.Join(Environment.NewLine, lines);
        return report;
    }
}

public class ReportDirector
{
    public void ConstructReport(IReportBuilder builder, ReportStyle style)
    {
        builder.SetStyle(style);

        builder.SetHeader("Monthly Report");
        builder.SetContent("This is main content of report.");

        builder.AddSection("Sales", "Sales increased by 10%.");
        builder.AddSection("Issues", "2 bugs found, 1 fixed.");

        builder.SetFooter("Generated by Builder Pattern");
    }
}

public class Program_BuilderPractice
{
    public static void Main()
    {
        var director = new ReportDirector();

        var style1 = new ReportStyle
        {
            BackgroundColor = "white",
            FontColor = "black",
            FontSize = 14,
            FontFamily = "Arial"
        };


        IReportBuilder textBuilder = new TextReportBuilder();
        director.ConstructReport(textBuilder, style1);
        Report textReport = textBuilder.GetReport();
        textReport.Export("report.txt");


        IReportBuilder htmlBuilder = new HtmlReportBuilder();
        director.ConstructReport(htmlBuilder, new ReportStyle
        {
            BackgroundColor = "#111",
            FontColor = "#eee",
            FontSize = 16,
            FontFamily = "Verdana"
        });
        Report htmlReport = htmlBuilder.GetReport();
        htmlReport.Export("report.html");

        IReportBuilder pdfBuilder = new PdfReportBuilder();
        director.ConstructReport(pdfBuilder, style1);
        Report pdfReport = pdfBuilder.GetReport();
        pdfReport.Export("report.pdf");

        Console.WriteLine("Reports exported: report.txt, report.html, report.pdf");
    }
}