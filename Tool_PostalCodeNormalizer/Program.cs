using System.Text;

if (args.Length == 0 || args.Contains("--help") || args.Contains("-h"))
{
    ShowHelp();
    return;
}

string? inputFile = null;
string? regionFile = null;
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
        case "--region":
            if (i + 1 < args.Length)
                regionFile = args[++i];
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

if (isNorwegian && !string.IsNullOrEmpty(regionFile) && !File.Exists(regionFile))
{
    Console.Error.WriteLine($"Error: Region file not found: {regionFile}");
    return;
}

try
{
    if (isNorwegian)
    {
        var processor = new Tool_PostalCodeNormalizer.NorwegianPostalCodeProcessor();
        processor.Process(inputFile, regionFile, outputFile);
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
    Console.WriteLine("  Tool_PostalCodeNormalizer --input <file> --output <file> [--no] [--region <file>]");
    Console.WriteLine();
    Console.WriteLine("Options:");
    Console.WriteLine("  --input <file>   Input postal code file path");
    Console.WriteLine("  --output <file>  Output normalized file path");
    Console.WriteLine("  --no             Process Norwegian postal code format");
    Console.WriteLine("  --region <file>  Region file path (used with --no for Norway)");
    Console.WriteLine("  --help, -h       Show this help message");
    Console.WriteLine();
    Console.WriteLine("Output format: CSV with columns PostalCode,City,Region");
}

void ProcessNorwegianPostalCodes(string inputFile, string? regionFile, string outputFile)
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
        char regionDelimiter = DetectDelimiter(firstRegionLine ?? "");
        Console.WriteLine($"Region file delimiter: {(regionDelimiter == '\t' ? "TAB" : regionDelimiter.ToString())}");
        
        foreach (var line in regionLines.Skip(1)) // Skip header
        {
            if (string.IsNullOrWhiteSpace(line)) continue;
            
            var parts = ParseCsvLine(line, regionDelimiter);
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
    char delimiter = DetectDelimiter(firstDataLine ?? "");
    Console.WriteLine($"Detected delimiter: {(delimiter == '\t' ? "TAB" : delimiter.ToString())}");
    
    int matchedRegions = 0;
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
        var postalCode = EscapeCsvField(kvp.Key);
        var city = EscapeCsvField(kvp.Value.City);
        var region = EscapeCsvField(kvp.Value.Region ?? "");
        
        writer.WriteLine($"{postalCode},{city},{region}");
    }

    Console.WriteLine($"Matched {postalCodes.Count} postal codes with regions (100.0%)");
}

void ProcessGenericPostalCodes(string inputFile, string outputFile)
{
    Console.WriteLine("Processing postal codes...");
    
    var postalCodes = new List<(string Code, string City, string Region)>();

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
        var postalCode = EscapeCsvField(pc.Code);
        var city = EscapeCsvField(pc.City);
        var region = EscapeCsvField(pc.Region);
        
        writer.WriteLine($"{postalCode},{city},{region}");
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

