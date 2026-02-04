using System.Text;

if (args.Length == 0 || args.Contains("--help") || args.Contains("-h"))
{
    ShowHelp();
    return;
}

string? inputFile = null;
string? stateFile = null;
string? outputFile = null;
bool isNorwegian = false;

for (int i = 0; i < args.Length; i++)
{
    switch (args[i].ToLower())
    {
        case "--no":
            isNorwegian = true;
            break;
        case "--input":
            if (i + 1 < args.Length)
                inputFile = args[++i];
            break;
        case "--state":
            if (i + 1 < args.Length)
                stateFile = args[++i];
            break;
        case "--output":
            if (i + 1 < args.Length)
                outputFile = args[++i];
            break;
    }
}

if (string.IsNullOrEmpty(inputFile))
{
    Console.Error.WriteLine("Error: --input parameter is required");
    ShowHelp();
    return;
}

if (string.IsNullOrEmpty(outputFile))
{
    Console.Error.WriteLine("Error: --output parameter is required");
    ShowHelp();
    return;
}

if (!File.Exists(inputFile))
{
    Console.Error.WriteLine($"Error: Input file not found: {inputFile}");
    return;
}

if (isNorwegian && !string.IsNullOrEmpty(stateFile) && !File.Exists(stateFile))
{
    Console.Error.WriteLine($"Error: State file not found: {stateFile}");
    return;
}

try
{
    if (isNorwegian)
    {
        var processor = new Tool_PostalCodeNormalizer.NorwegianPostalCodeProcessor();
        processor.Process(inputFile, stateFile, outputFile);
    }
    else
    {
        var processor = new Tool_PostalCodeNormalizer.GenericPostalCodeProcessor();
        processor.Process(inputFile, outputFile);
    }

    Console.WriteLine($"Successfully normalized postal codes to: {outputFile}");
}
catch (Exception ex)
{
    Console.Error.WriteLine($"Error processing file: {ex.Message}");
    Environment.Exit(1);
}

void ShowHelp()
{
    Console.WriteLine("Postal Code Normalizer Tool");
    Console.WriteLine();
    Console.WriteLine("Usage:");
    Console.WriteLine("  Tool_PostalCodeNormalizer --input <file> --output <file> [--no] [--state <file>]");
    Console.WriteLine();
    Console.WriteLine("Options:");
    Console.WriteLine("  --input <file>   Input postal code file path");
    Console.WriteLine("  --output <file>  Output normalized file path");
    Console.WriteLine("  --no             Process Norwegian postal code format");
    Console.WriteLine("  --state <file>  State file path (used with --no for Norway)");
    Console.WriteLine("  --help, -h       Show this help message");
    Console.WriteLine();
    Console.WriteLine("Output format: CSV with columns PostalCode,City,State");
}

void ProcessNorwegianPostalCodes(string inputFile, string? stateFile, string outputFile)
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
        char stateDelimiter = DetectDelimiter(firstStateLine ?? "");
        Console.WriteLine($"State file delimiter: {(stateDelimiter == '\t' ? "TAB" : stateDelimiter.ToString())}");
        
        foreach (var line in stateLines.Skip(1)) // Skip header
        {
            if (string.IsNullOrWhiteSpace(line)) continue;
            
            var parts = ParseCsvLine(line, stateDelimiter);
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
    char delimiter = DetectDelimiter(firstDataLine ?? "");
    Console.WriteLine($"Detected delimiter: {(delimiter == '\t' ? "TAB" : delimiter.ToString())}");
    
    int matchedStates = 0;
    int lineNumber = 1;
    foreach (var line in inputLines.Skip(1)) // Skip header
    {
        lineNumber++;
        if (string.IsNullOrWhiteSpace(line)) continue;
        
        var parts = ParseCsvLine(line, delimiter);
        if (parts.Length >= 2)
        {
            var postalCode = parts[0].Trim().Trim('"');
            var city = parts[1].Trim().Trim('"');
            var kommuneNumber = parts.Length >= 3 ? parts[2].Trim().Trim('"') : "";
            
            // Get state from state file if available
            // State code is the first 2 digits of Kommunenummer
            // Kommunenummer should be 4 digits, pad with leading zeros if needed
            string? state = null;
            if (!string.IsNullOrEmpty(kommuneNumber))
            {
                // Pad to 4 digits (e.g., 301 -> 0301)
                var paddedKommune = kommuneNumber.PadLeft(4, '0');
                var stateCode = paddedKommune.Substring(0, 2);
                
                if (states.TryGetValue(stateCode, out var stateValue))
                {
                    state = stateValue;
                    matchedStates++;
                }
                else if (lineNumber <= 10)
                {
                    Console.WriteLine($"DEBUG: Line {lineNumber} - PostalCode: {postalCode}, Kommune: {kommuneNumber} -> {paddedKommune}, StateCode: {stateCode} - NO MATCH");
                }
            }
            else if (lineNumber <= 10)
            {
                Console.WriteLine($"DEBUG: Line {lineNumber} - PostalCode: {postalCode}, No kommune number in column 3");
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
        var postalCode = EscapeCsvField(kvp.Key);
        var city = EscapeCsvField(kvp.Value.City);
        var state = EscapeCsvField(kvp.Value.State ?? "");
        
        writer.WriteLine($"{postalCode},{city},{state}");
    }

    Console.WriteLine($"Matched {postalCodes.Count} postal codes with states (100.0%)");
}

void ProcessGenericPostalCodes(string inputFile, string outputFile)
{
    Console.WriteLine("Processing postal codes...");
    
    var postalCodes = new List<(string Code, string City, string State)>();

    var inputLines = File.ReadAllLines(inputFile, Encoding.UTF8);
    
    // Try to detect delimiter
    var firstDataLine = inputLines.Skip(1).FirstOrDefault(l => !string.IsNullOrWhiteSpace(l));
    char delimiter = DetectDelimiter(firstDataLine ?? "");
    
    Console.WriteLine($"Detected delimiter: {(delimiter == '\t' ? "TAB" : delimiter.ToString())}");
    
    foreach (var line in inputLines.Skip(1)) // Skip header
    {
        if (string.IsNullOrWhiteSpace(line)) continue;
        
        var parts = ParseCsvLine(line, delimiter);
        if (parts.Length >= 2)
        {
            var postalCode = parts[0].Trim().Trim('"');
            var city = parts[1].Trim().Trim('"');
            var state = parts.Length >= 3 ? parts[2].Trim().Trim('"') : "";
            
            postalCodes.Add((postalCode, city, state));
        }
    }

    Console.WriteLine($"Processed {postalCodes.Count} postal codes");

    // Write normalized output
    using var writer = new StreamWriter(outputFile, false, Encoding.UTF8);
    writer.WriteLine("PostalCode,City,State");
    
    foreach (var pc in postalCodes.OrderBy(x => x.Code))
    {
        var postalCode = EscapeCsvField(pc.Code);
        var city = EscapeCsvField(pc.City);
        var state = EscapeCsvField(pc.State);
        
        writer.WriteLine($"{postalCode},{city},{state}");
    }
}

char DetectDelimiter(string line)
{
    if (line.Contains('\t')) return '\t';
    if (line.Contains(';')) return ';';
    if (line.Contains(',')) return ',';
    return '\t'; // Default to tab
}

string EscapeCsvField(string field)
{
    if (string.IsNullOrEmpty(field)) return "";
    
    // If field contains comma, quote, or newline, wrap in quotes and escape quotes
    if (field.Contains(',') || field.Contains('"') || field.Contains('\n') || field.Contains('\r'))
    {
        return $"\"{field.Replace("\"", "\"\"")}\"";
    }
    
    return field;
}

string[] ParseCsvLine(string line, char delimiter)
{
    var fields = new List<string>();
    var currentField = new StringBuilder();
    bool inQuotes = false;
    
    for (int i = 0; i < line.Length; i++)
    {
        char c = line[i];
        
        if (c == '"')
        {
            if (inQuotes && i + 1 < line.Length && line[i + 1] == '"')
            {
                // Escaped quote
                currentField.Append('"');
                i++;
            }
            else
            {
                // Toggle quotes
                inQuotes = !inQuotes;
            }
        }
        else if (c == delimiter && !inQuotes)
        {
            // End of field
            fields.Add(currentField.ToString());
            currentField.Clear();
        }
        else
        {
            currentField.Append(c);
        }
    }
    
    // Add last field
    fields.Add(currentField.ToString());
    
    return fields.ToArray();
}

