// ============================================================================
// SWE5301 Week 1 - Tests for Part A: the weather_summary.py port
//
// These tests run your whole program and compare what it prints with what the
// Python version prints. They don't care how you structured your classes.
//
// Setup (once):
//   1. Add this file to your Week01.Tests project.
//   2. The test project must reference your port project, e.g.
//        dotnet add tests/Week01.Tests reference src/Week01.Weather
//   3. If your port project is NOT called Week01.Weather, change
//      PortProjectName below to its name. This is the only line you may edit.
//   4. Run the tests: dotnet test, or Test -> Run All Tests in Visual Studio.
//
// Don't edit the tests themselves. Green means your port behaves like the Python.
// If [Fact] is underlined in red, add "using Xunit;" at the top of this file.
// ============================================================================

using System.Globalization;
using System.Reflection;

namespace Week01.Tests;

public class WeatherSummaryTests
{
    private const string PortProjectName = "Week01.Weather";

    // Exactly what weather_summary.py prints.
    private const string ExpectedOutput = """
        Station    Alt (m)  Avg temp  Rain (mm)  Feel
        Bolton         105      11.2       11.4  mild
        Leeds           63      14.8        3.8  mild
        Buxton         307       4.4       19.2  cold

        Warmest station: Leeds
        Wettest day in Buxton: Tue (9.4 mm)
        No data for York
        """;

    // The port is run once, and every test checks the same captured output.
    private static readonly Lazy<string> Output = new(RunPort);

    [Fact]
    public void Output_MatchesThePythonVersionExactly()
    {
        Assert.Equal(Normalise(ExpectedOutput), Output.Value);
    }

    // The tests below check one line at a time, so you can see which parts already work.

    [Fact]
    public void Output_HasTheTableHeading()
    {
        AssertHasLine("Station    Alt (m)  Avg temp  Rain (mm)  Feel");
    }

    [Theory]
    [InlineData("Bolton         105      11.2       11.4  mild")]
    [InlineData("Leeds           63      14.8        3.8  mild")]
    [InlineData("Buxton         307       4.4       19.2  cold")]
    public void Output_HasTheCorrectRowForEachStation(string expectedRow)
    {
        AssertHasLine(expectedRow);
    }

    [Fact]
    public void Output_NamesTheWarmestStation()
    {
        AssertHasLine("Warmest station: Leeds");
    }

    [Fact]
    public void Output_ReportsTheWettestDayForAKnownStation()
    {
        AssertHasLine("Wettest day in Buxton: Tue (9.4 mm)");
    }

    [Fact]
    public void Output_ReportsMissingDataForAnUnknownStation()
    {
        AssertHasLine("No data for York");
    }

    private static void AssertHasLine(string expectedLine)
    {
        string[] lines = Output.Value.Split('\n');
        Assert.True(Array.IndexOf(lines, expectedLine) >= 0,
            $"Expected this line in the output:{Environment.NewLine}{expectedLine}" +
            $"{Environment.NewLine}{Environment.NewLine}Actual output:{Environment.NewLine}{Output.Value}");
    }

    // Runs the port's entry point (its Main, or its top-level statements) and
    // captures everything it writes to the console.
    private static string RunPort()
    {
        Assembly port;
        try
        {
            port = Assembly.Load(new AssemblyName(PortProjectName));
        }
        catch (FileNotFoundException)
        {
            throw new InvalidOperationException(
                $"Can't find the project '{PortProjectName}'. Check that the test project references " +
                "your port project, and that PortProjectName matches its name.");
        }

        MethodInfo entryPoint = port.EntryPoint
            ?? throw new InvalidOperationException($"'{PortProjectName}' has no entry point. Is it a console app?");

        TextWriter originalOut = Console.Out;
        CultureInfo originalCulture = CultureInfo.CurrentCulture;
        var captured = new StringWriter();
        try
        {
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("en-GB");   // "." as decimal point
            Console.SetOut(captured);

            object?[]? arguments = entryPoint.GetParameters().Length == 0 ? null : [Array.Empty<string>()];
            object? result = entryPoint.Invoke(null, arguments);
            if (result is Task task)
            {
                task.GetAwaiter().GetResult();   // supports an async Main
            }
        }
        finally
        {
            Console.SetOut(originalOut);
            CultureInfo.CurrentCulture = originalCulture;
        }
        return Normalise(captured.ToString());
    }

    // Ignore Windows vs Linux line endings and a trailing newline.
    private static string Normalise(string text) => text.Replace("\r\n", "\n").TrimEnd('\n');
}
