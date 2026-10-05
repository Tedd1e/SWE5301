using Week1.Core;

/*
1. Where did Python's None force you to make a decision in C#?
   I used nullable types like double? and Reading? where the Python
   code would return None. This makes it explicit in C# that the method
   cant not return a normal value.

2. Which Python classes became records and which became classes? Why?
   Reading became a record because it is used to store fixed data
   and the Python version uses @dataclass(frozen=True).
   Station stayed as a class because it has a changing state, including
   its list of readings, and contains methods that operate on that state.

3. Which mistakes did the C# compiler catch that Python would only have
   found when the code ran?
   The C# compiler caught issues such as assigning null to a non nullable
   type, using wrong data types, and accessing nullable values without
   checking it first. Python would generally only reveal these problems
   when you run the code.
*/

internal class Program
{
    // static: Main belongs to the class itself, so the runtime can call it
    // without creating a Program object first.
    // string[] args: any command-line arguments, e.g. dotnet run -- hello
    private static void Main(string[] args)
    {
        Dictionary<string, int> altitudes = new Dictionary<string, int>()
        {
            { "Bolton", 105 },
            { "Leeds", 63 },
            { "Buxton", 307 }
        };

        List<Reading> readings = Load_readings();

        Dictionary<string, Station> stations =
            Build_stations(readings, altitudes);

        Console.WriteLine(
            $"{"Station",-10}" +
            $"{"Alt (m)",8}" +
            $"{"Avg temp",10}" +
            $"{"Rain (mm)",11}  Feel"
        );

        foreach (KeyValuePair<string, Station> item in stations)
        {
            string name = item.Key;
            Station station = item.Value;

            float? avg = station.average_temperature();

            if (avg != null)
            {
                Console.WriteLine(
                    $"{name,-10}" +
                    $"{station.Altitude_m,8}" +
                    $"{avg.Value,10:F1}" +
                    $"{station.total_rainfall(),11:F1}  " +
                    $"{Describe(avg.Value)}"
                );
            }
        }

        Station? warmest = null;

        foreach (Station station in stations.Values)
        {
            if (warmest == null ||
                station.average_temperature() > warmest.average_temperature())
            {
                warmest = station;
            }
        }

        Console.WriteLine();

        if (warmest != null)
        {
            Console.WriteLine($"Warmest station: {warmest.Name}");
        }

        string[] names = { "Buxton", "York" };

        foreach (string name in names)
        {
            Station? station = null;

            if (stations.ContainsKey(name))
            {
                station = stations[name];
            }

            if (station == null)
            {
                Console.WriteLine($"No data for {name}");
            }
            else
            {
                Reading? day = station.wettest_day();

                if (day != null)
                {
                    Console.WriteLine(
                        $"Wettest day in {name}: " +
                        $"{day.day} ({day.rainfall_mm} mm)"
                    );
                }
            }
        }
    }

    private static List<Reading> Load_readings()
    {
        return new List<Reading>()
        {
            new Reading("Bolton", "Mon", 11.2f, 4.1f),
            new Reading("Bolton", "Tue", 12.8f, 0.0f),
            new Reading("Bolton", "Wed", 9.5f, 7.3f),

            new Reading("Leeds", "Mon", 13.1f, 1.2f),
            new Reading("Leeds", "Tue", 15.4f, 0.0f),
            new Reading("Leeds", "Wed", 16.0f, 2.6f),

            new Reading("Buxton", "Mon", 4.2f, 6.8f),
            new Reading("Buxton", "Tue", 3.9f, 9.4f),
            new Reading("Buxton", "Wed", 5.1f, 3.0f)
        };
    }

    private static string Describe(float temperature)
    {
        if (temperature < 5)
        {
            return "cold";
        }
        else if (temperature < 15)
        {
            return "mild";
        }
        else
        {
            return "warm";
        }
    }

    private static Dictionary<string, Station> Build_stations(
        List<Reading> readings,
        Dictionary<string, int> altitudes)
    {
        Dictionary<string, Station> stations =
            new Dictionary<string, Station>();

        foreach (Reading reading in readings)
        {
            if (!stations.ContainsKey(reading.station))
            {
                int altitude = 0;

                if (altitudes.ContainsKey(reading.station))
                {
                    altitude = altitudes[reading.station];
                }

                stations[reading.station] =
                    new Station(reading.station, altitude);
            }

            stations[reading.station].add(reading);
        }

        return stations;
    }
}