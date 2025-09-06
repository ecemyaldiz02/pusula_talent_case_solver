using System;
using System.Linq;
using System.Xml.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

public static class FilterPeopleFromXmlSolver
{
    private record PersonRow(string Name, int Age, string Department, decimal Salary, DateTime HireDate);
    private record PeopleReport(
      [property: JsonPropertyName("Names")] System.Collections.Generic.List<string> Names,
      [property: JsonPropertyName("TotalSalary")] decimal TotalSalary,
      [property: JsonPropertyName("AverageSalary")] decimal AverageSalary,
      [property: JsonPropertyName("MaxSalary")] decimal MaxSalary,
      [property: JsonPropertyName("Count")] int Count
    );

    private static DateTime ParseDate(string? dateStr) => DateTime.TryParse(dateStr, out var d) ? d : DateTime.MinValue;

    public static string FilterPeopleFromXml(string xmlData)
    {
        var jsonOptions = new JsonSerializerOptions { WriteIndented = true };
        if (string.IsNullOrWhiteSpace(xmlData))
        {
            return JsonSerializer.Serialize(new PeopleReport(new System.Collections.Generic.List<string>(), 0m, 0m, 0m, 0), jsonOptions);
        }

        var doc = XDocument.Parse(xmlData);
        var rows = doc.Descendants("Person")
          .Select(p => new PersonRow(
            (string?)p.Element("Name") ?? string.Empty,
            (int?)p.Element("Age") ?? 0,
            (string?)p.Element("Department") ?? string.Empty,
            (decimal?)p.Element("Salary") ?? 0m,
            ParseDate((string?)p.Element("HireDate"))
          ))
          .Where(x => x.Age > 30 && x.Department == "IT" && x.Salary > 5000m && x.HireDate < new DateTime(2019, 1, 1))
          .ToList();

        var names = rows.Select(x => x.Name).OrderBy(n => n).ToList();
        decimal total = rows.Sum(x => x.Salary);
        decimal avg = rows.Count > 0 ? Math.Round(rows.Average(x => x.Salary), 2) : 0m;
        decimal max = rows.Count > 0 ? rows.Max(x => x.Salary) : 0m;
        int count = rows.Count;

        var report = new PeopleReport(names, total, avg, max, count);
        return JsonSerializer.Serialize(report, jsonOptions);
    }
}


