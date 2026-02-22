using System;

public class Director
{
    public Report BuildReport(IReportBuilder builder, string header, string content, string footer)
    {
        builder.SetHeader(header);
        builder.SetContent(content);
        builder.SetFooter(footer);
        return builder.GetReport();
    }
}

