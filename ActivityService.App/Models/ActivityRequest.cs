using Microsoft.AspNetCore.Http;

namespace ActivityService.App.Models
{
    public class ActivityRequest
    {
    public Guid? Id { get; set; }

    public Guid UserId { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public double Latitude { get; set; }

    public double Longitude { get; set; }

    public List<IFormFile> ImageFiles { get; set; } = new List<IFormFile>();
    }
}