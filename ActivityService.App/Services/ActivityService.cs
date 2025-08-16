using Amazon.S3;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ActivityService.App.Interfaces;
using ActivityService.App.Models;
using ActivityService.Domain;
using ActivityService.Domain.Entities;

namespace ActivityService.App.Services;

public class ActivityService : IActivityService
{
    private readonly ActivityDbContext FContext;
    private readonly IAmazonS3 _s3Client;
    private readonly string _bucketName;
    private readonly string _serviceUrl;

    public ActivityService(ActivityDbContext context, IConfiguration configuration)
    {
        FContext = context;
        
        // Конфигурация для TimeWeb Cloud
        _bucketName = configuration["TimeWeb:BucketName"];
        _serviceUrl = configuration["TimeWeb:ServiceURL"];
        
        // Настройка S3 клиента для TimeWeb Cloud
        var config = new AmazonS3Config
        {
            ServiceURL = _serviceUrl,
            ForcePathStyle = true
        };
        
        _s3Client = new AmazonS3Client(
            configuration["TimeWeb:AccessKey"],
            configuration["TimeWeb:SecretKey"],
            config
        );
    }

    public async Task<ActivityResponse> AddUserActivityAsync(ActivityRequest ActivityRequest)
    {
        var imageUrls = new List<string>();
        foreach (var imageFile in ActivityRequest.ImageFiles)
        {
            var imageUrl = await UploadFileToTimeWebAsync(imageFile);
            imageUrls.Add(imageUrl);
        }

        var activity = ActivityRequest.Adapt<Activity>();
        activity.ImagesUrls = imageUrls;
        await FContext.Activities.AddAsync(activity);
        await FContext.SaveChangesAsync();

        return new ActivityResponse
        {
            Id = activity.Id,
            UserId = activity.UserId,
            Latitude = activity.Latitude,
            Longitude = activity.Longitude,
            Title = activity.Title,
            Description = activity.Description,
            LikesCount = activity.LikesCount,
            CommentsCount = activity.CommentsCount,
            CreatedAt = activity.CreatedAt,
            ImagesUrls = activity.ImagesUrls
        };
    }

    private async Task<string> UploadFileToTimeWebAsync(IFormFile file)
    {
        var fileKey = $"user-activities/{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";

        using (var stream = file.OpenReadStream())
        {
            var request = new Amazon.S3.Model.PutObjectRequest
            {
                BucketName = _bucketName,
                Key = fileKey,
                InputStream = stream,
                ContentType = file.ContentType,
                CannedACL = S3CannedACL.PublicRead
            };

            await _s3Client.PutObjectAsync(request);
        }
        return $"{_serviceUrl}/{_bucketName}/{fileKey}";
    }
    public async Task<List<ActivityResponse>?> GetAllUserActivitiesAsync(Guid userId)
    {
        return FContext.Activities
            .Where(p => p.UserId == userId).Adapt<List<ActivityResponse>?>();
    }
}