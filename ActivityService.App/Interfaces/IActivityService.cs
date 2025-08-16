using ActivityService.App.Models;

namespace ActivityService.App.Interfaces;

public interface IActivityService
{
    Task<ActivityResponse> AddUserActivityAsync(ActivityRequest userActivity);
    Task<List<ActivityResponse>?> GetAllUserActivitiesAsync(Guid userId);
}