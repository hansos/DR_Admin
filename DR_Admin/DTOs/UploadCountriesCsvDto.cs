using Microsoft.AspNetCore.Http;

namespace ISPAdmin.DTOs
{
    /// <summary>
    /// Data transfer object for uploading a CSV file with countries
    /// </summary>
    public class UploadCountriesCsvDto
    {
        /// <summary>
        /// Gets or sets the CSV file to upload
        /// </summary>
        public IFormFile? File { get; set; }
    }
}

