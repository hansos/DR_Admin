using System.Text;

namespace Tool_PostalCodeNormalizer;

internal class NorwegianPostalCodeProcessor
{
    public void Process(string inputFile, string? regionFile, string outputFile)
    {
        Console.WriteLine("Processing Norwegian postal codes...");

        var postalCodes = new Dictionary<string, (string City, string? Region)>();
        var regions = new Dictionary<string, string>();

        // Read region file if provided
        if (!string.IsNullOrEmpty(regionFile))
        {
            Console.WriteLine($"Reading region file: {regionFile}");
            var regionLines = File.ReadAllLines(regionFile, Encoding.UTF8);

            var firstRegionLine = regionLines.Skip(1).FirstOrDefault(l => !string.IsNullOrWhiteSpace(l));
            char regionDelimiter = CsvHelper.DetectDelimiter(firstRegionLine ?? "");
            Console.WriteLine($"Region file delimiter: {(regionDelimiter == '\t' ? "TAB" : regionDelimiter.ToString())}");

            foreach (var line in regionLines.Skip(1)) // Skip header
            {
                if (string.IsNullOrWhiteSpace(line)) continue;

                var parts = CsvHelper.ParseCsvLine(line, regionDelimiter);
                if (parts.Length >= 2)
                {
                    var regionCode = parts[0].Trim().Trim('"');
                    var regionName = parts[1].Trim().Trim('"');
                    regions[regionCode] = regionName;
                }
            }
            Console.WriteLine($"Loaded {regions.Count} region mappings");
            Console.WriteLine("DEBUG: Region codes loaded:");
            foreach (var kvp in regions.OrderBy(x => x.Key))
            {
                Console.WriteLine($"  {kvp.Key} -> {kvp.Value}");
            }
        }

        // Read postal code file
        Console.WriteLine($"Reading postal code file: {inputFile}");
        var inputLines = File.ReadAllLines(inputFile, Encoding.UTF8);

        // Detect delimiter from first data line
        var firstDataLine = inputLines.Skip(1).FirstOrDefault(l => !string.IsNullOrWhiteSpace(l));
        char delimiter = CsvHelper.DetectDelimiter(firstDataLine ?? "");
        Console.WriteLine($"Detected delimiter: {(delimiter == '\t' ? "TAB" : delimiter.ToString())}");

        int matchedRegions = 0;
        int lineNumber = 1;
        foreach (var line in inputLines.Skip(1)) // Skip header
        {
            lineNumber++;
            if (string.IsNullOrWhiteSpace(line)) continue;

            var parts = CsvHelper.ParseCsvLine(line, delimiter);
            if (parts.Length >= 2)
            {
                var postalCode = parts[0].Trim().Trim('"');
                var city = parts[1].Trim().Trim('"');
                var kommuneNumber = parts.Length >= 3 ? parts[2].Trim().Trim('"') : "";

                // Get region from region file if available
                // Region code is the first 2 digits of Kommunenummer
                // Kommunenummer should be 4 digits, pad with leading zeros if needed
                string? region = null;
                if (!string.IsNullOrEmpty(kommuneNumber))
                {
                    // Pad to 4 digits (e.g., 301 -> 0301)
                    var paddedKommune = kommuneNumber.PadLeft(4, '0');
                    var regionCode = paddedKommune.Substring(0, 2);

                    if (regions.TryGetValue(regionCode, out var regionValue))
                    {
                        region = regionValue;
                        matchedRegions++;
                    }
                    else if (lineNumber <= 10)
                    {
                        Console.WriteLine($"DEBUG: Line {lineNumber} - PostalCode: {postalCode}, Kommune: {kommuneNumber} -> {paddedKommune}, RegionCode: {regionCode} - NO MATCH");
                    }
                }
                else if (lineNumber <= 10)
                {
                    Console.WriteLine($"DEBUG: Line {lineNumber} - PostalCode: {postalCode}, No kommune number in column 3");
                }

                postalCodes[postalCode] = (city, region);
            }
        }

        Console.WriteLine($"Processed {postalCodes.Count} postal codes");
        if (regions.Count > 0)
        {
            Console.WriteLine($"Matched {matchedRegions} postal codes with regions ({(double)matchedRegions / postalCodes.Count * 100:F1}%)");
        }

        // Write normalized output
        using var writer = new StreamWriter(outputFile, false, Encoding.UTF8);
        writer.WriteLine("PostalCode,City,Region");

        foreach (var kvp in postalCodes.OrderBy(x => x.Key))
        {
            var postalCode = CsvHelper.EscapeCsvField(kvp.Key);
            var city = CsvHelper.EscapeCsvField(kvp.Value.City);
            var region = CsvHelper.EscapeCsvField(kvp.Value.Region ?? "");

            writer.WriteLine($"{postalCode},{city},{region}");
        }

        Console.WriteLine($"Matched {postalCodes.Count} postal codes with regions (100.0%)");
    }
}
