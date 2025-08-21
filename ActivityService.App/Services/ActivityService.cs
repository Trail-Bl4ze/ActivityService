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

    public ActivityService(ActivityDbContext context, IConfiguration configuration)
    {
        FContext = context;
        
    }

    public async Task<ActivityResponse> AddUserActivityAsync(ActivityRequest ActivityRequest, CancellationToken stoppingToken)
    {
        var activity = ActivityRequest.Adapt<Activity>();
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

    public async Task<List<ActivityResponse>?> GetAllUserActivitiesAsync(Guid userId, CancellationToken stoppingToken)
    {
        return FContext.Activities
            .Where(p => p.UserId == userId).Adapt<List<ActivityResponse>?>();
    }
}