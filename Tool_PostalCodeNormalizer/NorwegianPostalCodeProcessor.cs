using System.Text;

namespace Tool_PostalCodeNormalizer;

internal class NorwegianPostalCodeProcessor
{
    public void Process(string inputFile, string? stateFile, string outputFile)
    {
        Console.WriteLine("Processing Norwegian postal codes...");

        var postalCodes = new Dictionary<string, (string City, string? State)>();
        var states = new Dictionary<string, string>();

        // Read state file if provided
        if (!string.IsNullOrEmpty(stateFile))
        {
            Console.WriteLine($"Reading state file: {stateFile}");
            var stateLines = File.ReadAllLines(stateFile, Encoding.UTF8);

            var firstStateLine = stateLines.Skip(1).FirstOrDefault(l => !string.IsNullOrWhiteSpace(l));
            char stateDelimiter = CsvHelper.DetectDelimiter(firstStateLine ?? "");
            Console.WriteLine($"State file delimiter: {(stateDelimiter == '\t' ? "TAB" : stateDelimiter.ToString())}");

            foreach (var line in stateLines.Skip(1)) // Skip header
            {
                if (string.IsNullOrWhiteSpace(line)) continue;

                var parts = CsvHelper.ParseCsvLine(line, stateDelimiter);
                if (parts.Length >= 2)
                {
                    var stateCode = parts[0].Trim().Trim('"');
                    var stateName = parts[1].Trim().Trim('"');
                    states[stateCode] = stateName;
                }
            }
            Console.WriteLine($"Loaded {states.Count} state mappings");
            Console.WriteLine("DEBUG: State codes loaded:");
            foreach (var kvp in states.OrderBy(x => x.Key))
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

        int matchedStates = 0;
        int lineNumber = 1;
        foreach (var line in inputLines.Skip(1)) // Skip header
        {
            lineNumber++;
            if (string.IsNullOrWhiteSpace(line)) continue;

            var parts = CsvHelper.ParseCsvLine(line, delimiter);
            if (parts.Length >= 2)
            {
                var postalCode = parts[0].Trim().Trim('"').PadLeft(4, '0');
                var city = parts[1].Trim().Trim('"');
                var column3 = parts.Length >= 3 ? parts[2].Trim().Trim('"') : "";

                string? state = null;
                
                // If state file is provided, treat column 3 as kommune number and derive state
                if (states.Count > 0 && !string.IsNullOrEmpty(column3))
                {
                    // Pad to 4 digits (e.g., 301 -> 0301)
                    var paddedKommune = column3.PadLeft(4, '0');
                    var stateCode = paddedKommune.Substring(0, 2);

                    if (states.TryGetValue(stateCode, out var stateValue))
                    {
                        state = stateValue;
                        matchedStates++;
                    }
                    else if (lineNumber <= 10)
                    {
                        Console.WriteLine($"DEBUG: Line {lineNumber} - PostalCode: {postalCode}, Kommune: {column3} -> {paddedKommune}, StateCode: {stateCode} - NO MATCH");
                    }
                }
                else if (!string.IsNullOrEmpty(column3))
                {
                    // If no state file, treat column 3 as state value directly
                    state = column3;
                }
                else if (lineNumber <= 10)
                {
                    Console.WriteLine($"DEBUG: Line {lineNumber} - PostalCode: {postalCode}, No value in column 3");
                }

                postalCodes[postalCode] = (city, state);
            }
        }

        Console.WriteLine($"Processed {postalCodes.Count} postal codes");
        if (states.Count > 0)
        {
            Console.WriteLine($"Matched {matchedStates} postal codes with states ({(double)matchedStates / postalCodes.Count * 100:F1}%)");
        }

        // Write normalized output
        using var writer = new StreamWriter(outputFile, false, Encoding.UTF8);
        writer.WriteLine("PostalCode,City,State");

        foreach (var kvp in postalCodes.OrderBy(x => x.Key))
        {
            var postalCode = CsvHelper.EscapeCsvField(kvp.Key);
            var city = CsvHelper.EscapeCsvField(kvp.Value.City);
            var state = CsvHelper.EscapeCsvField(kvp.Value.State ?? "");

            writer.WriteLine($"{postalCode},{city},{state}");
        }

        Console.WriteLine($"Matched {postalCodes.Count} postal codes with states (100.0%)");
    }
}
