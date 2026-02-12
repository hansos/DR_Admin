using EmailSenderLib.Templating;
using EmailSenderLib.Templating.Models;
using EmailSenderLib.Enums;
using Microsoft.Extensions.Caching.Memory;
using Xunit;

namespace EmailSenderLib.Tests;

/// <summary>
/// Example unit tests for the email template system
/// </summary>
public class TemplateSystemTests
{
    [Fact]
    public void TemplateRenderer_ShouldReplacePlaceholders()
    {
        // Arrange
        var template = "Hello {{Name}}, your order {{OrderId}} is ready!";
        var values = new Dictionary<string, object>
        {
            { "Name", "John" },
            { "OrderId", "12345" }
        };

        // Act
        var result = TemplateRenderer.Render(template, values);

        // Assert
        Assert.Equal("Hello John, your order 12345 is ready!", result);
    }

    [Fact]
    public void TemplateRenderer_ShouldHandleMissingPlaceholders()
    {
        // Arrange
        var template = "Hello {{Name}}, your order {{OrderId}} is ready!";
        var values = new Dictionary<string, object>
        {
            { "Name", "John" }
            // OrderId is missing
        };

        // Act
        var result = TemplateRenderer.Render(template, values);

        // Assert - missing placeholders remain unchanged
        Assert.Equal("Hello John, your order {{OrderId}} is ready!", result);
    }

    [Fact]
    public void TemplateRenderer_ShouldRenderWithMaster()
    {
        // Arrange
        var masterTemplate = "<html><body>{{Content}}</body></html>";
        var contentTemplate = "<h1>Hello {{Name}}</h1>";
        var values = new Dictionary<string, object>
        {
            { "Name", "World" }
        };

        // Act
        var result = TemplateRenderer.RenderWithMaster(contentTemplate, masterTemplate, values);

        // Assert
        Assert.Equal("<html><body><h1>Hello World</h1></body></html>", result);
    }

    [Fact]
    public void TemplateRenderer_ModelToDictionary_ShouldConvertProperties()
    {
        // Arrange
        var model = new DomainRegisteredModel
        {
            DomainName = "example.com",
            RegistrationDate = "2026-01-15",
            ExpirationDate = "2027-01-15",
            AutoRenew = "Enabled",
            CustomerPortalUrl = "https://portal.example.com"
        };

        // Act
        var dict = TemplateRenderer.ModelToDictionary(model);

        // Assert
        Assert.Equal(5, dict.Count);
        Assert.Equal("example.com", dict["DomainName"]);
        Assert.Equal("2026-01-15", dict["RegistrationDate"]);
        Assert.Equal("2027-01-15", dict["ExpirationDate"]);
        Assert.Equal("Enabled", dict["AutoRenew"]);
        Assert.Equal("https://portal.example.com", dict["CustomerPortalUrl"]);
    }

    [Fact]
    public void MessagingService_ShouldRenderDomainRegisteredEmail()
    {
        // Arrange
        var cache = new MemoryCache(new MemoryCacheOptions());
        
        // Create temporary test templates
        var testTemplatesPath = Path.Combine(Path.GetTempPath(), "TestTemplates");
        Directory.CreateDirectory(testTemplatesPath);
        Directory.CreateDirectory(Path.Combine(testTemplatesPath, "Layouts"));
        Directory.CreateDirectory(Path.Combine(testTemplatesPath, "TestMessage"));

        try
        {
            // Create master template
            File.WriteAllText(
                Path.Combine(testTemplatesPath, "Layouts", "email.html.master.txt"),
                "<html><body>{{Content}}</body></html>"
            );

            // Create message template
            File.WriteAllText(
                Path.Combine(testTemplatesPath, "TestMessage", "email.html.txt"),
                "<h1>Domain: {{DomainName}}</h1><p>Date: {{RegistrationDate}}</p>"
            );

            var loader = new TemplateLoader(cache, testTemplatesPath);
            var service = new MessagingService(loader);

            var model = new DomainRegisteredModel
            {
                DomainName = "test.com",
                RegistrationDate = "2026-01-15",
                ExpirationDate = "2027-01-15",
                AutoRenew = "Enabled",
                CustomerPortalUrl = "https://portal.example.com"
            };

            // Act
            var result = service.RenderMessage("TestMessage", MessageChannel.EmailHtml, model);

            // Assert
            Assert.Contains("test.com", result);
            Assert.Contains("2026-01-15", result);
            Assert.Contains("<html>", result);
            Assert.Contains("</html>", result);
        }
        finally
        {
            // Cleanup
            if (Directory.Exists(testTemplatesPath))
            {
                Directory.Delete(testTemplatesPath, true);
            }
        }
    }

    [Fact]
    public void TemplateLoader_ShouldCacheTemplates()
    {
        // Arrange
        var cache = new MemoryCache(new MemoryCacheOptions());
        var testTemplatesPath = Path.Combine(Path.GetTempPath(), "TestTemplatesCache");
        Directory.CreateDirectory(testTemplatesPath);
        Directory.CreateDirectory(Path.Combine(testTemplatesPath, "TestMessage"));

        try
        {
            var templatePath = Path.Combine(testTemplatesPath, "TestMessage", "email.html.txt");
            File.WriteAllText(templatePath, "Original Content");

            var loader = new TemplateLoader(cache, testTemplatesPath);

            // Act - First load (from file)
            var firstLoad = loader.LoadTemplate("TestMessage", "email.html");
            
            // Change file content
            File.WriteAllText(templatePath, "Changed Content");
            
            // Second load (should be from cache, not file)
            var secondLoad = loader.LoadTemplate("TestMessage", "email.html");

            // Assert - Both should return the cached version
            Assert.Equal("Original Content", firstLoad);
            Assert.Equal("Original Content", secondLoad); // Still original because cached
        }
        finally
        {
            if (Directory.Exists(testTemplatesPath))
            {
                Directory.Delete(testTemplatesPath, true);
            }
        }
    }

    [Fact]
    public void TemplateLoader_ShouldThrowExceptionForMissingTemplate()
    {
        // Arrange
        var cache = new MemoryCache(new MemoryCacheOptions());
        var testTemplatesPath = Path.Combine(Path.GetTempPath(), "TestTemplatesMissing");
        Directory.CreateDirectory(testTemplatesPath);

        try
        {
            var loader = new TemplateLoader(cache, testTemplatesPath);

            // Act & Assert
            Assert.Throws<FileNotFoundException>(() => 
                loader.LoadTemplate("NonExistentMessage", "email.html")
            );
        }
        finally
        {
            if (Directory.Exists(testTemplatesPath))
            {
                Directory.Delete(testTemplatesPath, true);
            }
        }
    }
}
