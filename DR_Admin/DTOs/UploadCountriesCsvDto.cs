using Microsoft.AspNetCore.Http;

namespace ISPAdmin.DTOs
{
    public class UploadCountriesCsvDto
    {
        public IFormFile? File { get; set; }
    }
}
