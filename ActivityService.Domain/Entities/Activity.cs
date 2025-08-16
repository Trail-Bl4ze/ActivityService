using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ActivityService.Domain.Entities;

[Table("activities", Schema = "activity")]
public class Activity
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("user_id")]
    public Guid UserId { get; set; }

    [Column("title")]
    public string? Title { get; set; }

    [Column("description")]
    public string? Description { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("latitude")]
    public double Latitude { get; set; }

    [Column("longitude")]
    public double Longitude { get; set; }

    [Column("likes_count")]
    public int? LikesCount = 0;

    [Column("comments_count")]
    public int? CommentsCount = 0;

    [Column("image_urls")]
    [Required]
    public List<string> ImagesUrls { get; set; }
}