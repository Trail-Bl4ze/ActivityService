// using System.Security.Claims;
// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Mvc;
// using ActivityService.App.Interfaces;
// using ActivityService.App.Models;

// namespace ActivityService.Controllers;

// [ApiController]
// [Route("api/[controller]")]
// public class UserActivitiesController : ControllerBase
// {
//     private readonly IActivityService FActivityService;

//     public UserActivitiesController(
//         IActivityService activityService)
//     {
//         FActivityService = activityService;
//     }

//     [HttpPost]
//     public async Task<IActionResult> AddActivity([FromForm] ActivityRequest activityDto)
//     {
//         try
//         {
//             var result = await FActivityService.AddUserActivityAsync(activityDto, new CancellationToken());
//             return Ok(result);
//         }
//         catch (Exception ex)
//         {
//             return BadRequest(ex.Message);
//         }
//     }

//     [HttpGet]
//     public async Task<IActionResult> GetAllActivities([FromQuery] Guid userId)
//     {
//         try
//         {
//             var activities = await FActivityService.GetAllUserActivitiesAsync(userId, new CancellationToken());
//             return Ok(activities);
//         }
//         catch (Exception ex)
//         {
//             return BadRequest(ex.Message);
//         }
//     }
// }