using Grpc.Core;
using ActivityService.Grpc;
using Google.Protobuf.WellKnownTypes;
using ActivityService.App.Interfaces;
using ActivityService.App.Models;

namespace ActivityService.Services;

public class ActivitiesGrpcService : ActivitiesGrpc.ActivitiesGrpcBase
{
    private readonly IActivityService _activityService;
    private readonly ILogger<ActivitiesGrpcService> _logger;

    public ActivitiesGrpcService(
        IActivityService activityService,
        ILogger<ActivitiesGrpcService> logger)
    {
        _activityService = activityService;
        _logger = logger;
    }

    public override async Task<UserActivitiesResponse> GetUserActivities(
        UserActivitiesRequest request,
        ServerCallContext context)
    {
        try
        {
            // Получаем данные из внутреннего сервиса
            var internalActivities = await _activityService.GetAllUserActivitiesAsync(
                Guid.Parse(request.UserId), 
                context.CancellationToken);
            
            // Преобразуем во внешний формат gRPC
            var response = new UserActivitiesResponse();
            response.Activities.AddRange(internalActivities.Select(ToGrpcResponse));
            
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetUserActivities");
            throw new RpcException(
                new Status(StatusCode.Internal, "Internal server error"));
        }
    }

    // public override async Task<ActivityResponse> GetActivityById(
    //     ActivityByIdRequest request,
    //     ServerCallContext context)
    // {
    //     try
    //     {
    //         var internalActivity = await _activityService.GetByIdAsync(Guid.Parse(request.Id));
            
    //         if (internalActivity == null)
    //         {
    //             throw new RpcException(
    //                 new Status(StatusCode.NotFound, "Activity not found"));
    //         }

    //         return ToGrpcResponse(internalActivity);
    //     }
    //     catch (Exception ex)
    //     {
    //         _logger.LogError(ex, "Error in GetActivityById");
    //         throw new RpcException(
    //             new Status(StatusCode.Internal, "Internal server error"));
    //     }
    // }

    private static ActivityService.Grpc.ActivityResponse ToGrpcResponse(ActivityService.App.Models.ActivityResponse internalModel)
    {
        var response = new ActivityService.Grpc.ActivityResponse()
        {
            Id = internalModel.Id.ToString(),
            UserId = internalModel.UserId.ToString(),
            Title = internalModel.Title,
            Description = internalModel.Description,
            Latitude = internalModel.Latitude,
            Longitude = internalModel.Longitude
        };

        foreach (var url in internalModel.ImagesUrls)
        {
            response.ImagesUrls.Add(url);
        }

        return response;
    }
}