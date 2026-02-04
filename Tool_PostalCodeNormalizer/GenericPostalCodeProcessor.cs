using System.Text;

namespace Tool_PostalCodeNormalizer;

internal class GenericPostalCodeProcessor
{
    public void Process(string inputFile, string outputFile)
    {
        Console.WriteLine("Processing postal codes...");

        var postalCodes = new List<(string Code, string City, string Region)>();

        var inputLines = File.ReadAllLines(inputFile, Encoding.UTF8);

        // Try to detect delimiter
        var firstDataLine = inputLines.Skip(1).FirstOrDefault(l => !string.IsNullOrWhiteSpace(l));
        char delimiter = CsvHelper.DetectDelimiter(firstDataLine ?? "");

        Console.WriteLine($"Detected delimiter: {(delimiter == '\t' ? "TAB" : delimiter.ToString())}");

        foreach (var line in inputLines.Skip(1)) // Skip header
        {
            if (string.IsNullOrWhiteSpace(line)) continue;

            var parts = CsvHelper.ParseCsvLine(line, delimiter);
            if (parts.Length >= 2)
            {
                var postalCode = parts[0].Trim().Trim('"');
                var city = parts[1].Trim().Trim('"');
                var region = parts.Length >= 3 ? parts[2].Trim().Trim('"') : "";

                postalCodes.Add((postalCode, city, region));
            }
        }

        Console.WriteLine($"Processed {postalCodes.Count} postal codes");

        // Write normalized output
        using var writer = new StreamWriter(outputFile, false, Encoding.UTF8);
        writer.WriteLine("PostalCode,City,Region");

        foreach (var pc in postalCodes.OrderBy(x => x.Code))
        {
            var postalCode = CsvHelper.EscapeCsvField(pc.Code);
            var city = CsvHelper.EscapeCsvField(pc.City);
            var region = CsvHelper.EscapeCsvField(pc.Region);

            writer.WriteLine($"{postalCode},{city},{region}");
        }
    }
}
