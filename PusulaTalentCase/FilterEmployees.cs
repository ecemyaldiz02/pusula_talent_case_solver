using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

public static class FilterEmployeesSolver
{
    private record Employee(string Name, int Age, string Department, decimal Salary, DateTime HireDate);
    private static readonly DateTime MinHireDate = new(2017, 1, 1);
    private record EmployeeReport(
      List<string> Names,
      decimal TotalSalary,
      decimal AverageSalary,
      decimal MinSalary,
      decimal MaxSalary,
      int Count
    );

    public static string FilterEmployees(IEnumerable<(string Name, int Age, string Department, decimal Salary, DateTime HireDate)> employees)
    {
        return BuildEmployeeReportJson(employees, null);
    }

    // İsteğe bağlı JSON seçenekleri ile çağrılabilen aşırı yükleme
    public static string FilterEmployees(IEnumerable<(string Name, int Age, string Department, decimal Salary, DateTime HireDate)> employees, JsonSerializerOptions? jsonOptions)
    {
        return BuildEmployeeReportJson(employees, jsonOptions);
    }

    private static string BuildEmployeeReportJson(IEnumerable<(string Name, int Age, string Department, decimal Salary, DateTime HireDate)> employees, JsonSerializerOptions? jsonOptions)
    {
        var mapped = MapToEmployees(employees);
        var filtered = ApplyFilters(mapped);
        if (!filtered.Any())
        {
            return JsonSerializer.Serialize(new EmployeeReport(new List<string>(), 0m, 0m, 0m, 0m, 0), jsonOptions);
        }

        var orderedNames = OrderNames(filtered);
        var stats = ComputeStatistics(filtered);

        var report = new EmployeeReport(orderedNames, stats.total, stats.avg, stats.min, stats.max, stats.count);

        return JsonSerializer.Serialize(report, jsonOptions);
    }

    private static IEnumerable<Employee> MapToEmployees(IEnumerable<(string Name, int Age, string Department, decimal Salary, DateTime HireDate)> employees)
    {
        return (employees ?? Enumerable.Empty<(string, int, string, decimal, DateTime)>())
          .Select(e => new Employee(e.Name, e.Age, e.Department, e.Salary, e.HireDate));
    }

    private static IEnumerable<Employee> ApplyFilters(IEnumerable<Employee> employees)
    {
        return employees.Where(e =>
          e.Age is >= 25 and <= 40 &&
          (e.Department is "IT" or "Finance") &&
          e.Salary is >= 5000m and <= 9000m &&
          e.HireDate > MinHireDate
        );
    }

    private static List<string> OrderNames(IEnumerable<Employee> employees)
    {
        return employees
          .Select(e => e.Name)
          .OrderByDescending(n => n?.Length ?? 0)
          .ThenBy(n => n)
          .ToList();
    }

    private static (decimal total, decimal avg, decimal min, decimal max, int count) ComputeStatistics(IEnumerable<Employee> employees)
    {
        var list = employees.ToList();
        int count = list.Count;
        decimal total = list.Sum(e => e.Salary);
        decimal avg = Math.Round(list.Average(e => e.Salary), 2);
        decimal min = list.Min(e => e.Salary);
        decimal max = list.Max(e => e.Salary);
        return (total, avg, min, max, count);
    }
}


