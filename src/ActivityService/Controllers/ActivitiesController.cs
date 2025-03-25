using ActivityService.Data;
using ActivityService.DTOs;
using ActivityService.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ActivityService.Controllers;

[ApiController]
[Route("api/activities")]
public class ActivitiesController : ControllerBase
{
    private readonly ActivityDbContext _context;
    private readonly IMapper _mapper;
    public ActivitiesController(ActivityDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<List<ActivityDto>>>GetAllActivities()
    {
        var activities = await _context.Activities
        .Include(x => x.Task)
        .OrderBy(x => x.Task.Name)
        .ToListAsync();

        return _mapper.Map<List<ActivityDto>>(activities);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ActivityDto>> GetActivity(Guid id)
    {
        var activity = await _context.Activities
        .Include(x => x.Task)
        .FirstOrDefaultAsync(x => x.Id == id);

        if(activity == null)
        {
            return NotFound();
        }

        return _mapper.Map<ActivityDto>(activity);
    }

    [HttpPost]
    public async Task<ActionResult<ActivityDto>> CreateActivity(CreateActivityDto activityDto)
    {
        var activity = _mapper.Map<Activity>(activityDto);
        //add currrent user as an author
        activity.Author = "test";

        _context.Activities.Add(activity);
        var result = await _context.SaveChangesAsync() > 0;

        if (!result)
        {
            return BadRequest("Could not save changes to the database");
        }

        return CreatedAtAction(nameof(GetActivity), new { id = activity.Id }, _mapper.Map<ActivityDto>(activity));
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateActivity(Guid id, UpdateActivityDto updateActivityDto)
    {
        var activity = await _context.Activities.Include(x => x.Task).FirstOrDefaultAsync(x => x.Id == id);

        if(activity == null)
        {
            return NotFound();
        }

        //check author ==username

        activity.Task.Name = updateActivityDto.Name ?? activity.Task.Name;
        activity.Task.Description = updateActivityDto.Description ?? activity.Task.Description;
        activity.Task.PredictedTime = updateActivityDto.PredictedTime ?? activity.Task.PredictedTime;
        activity.Task.Category = updateActivityDto.Category ?? activity.Task.Category;
        activity.Task.Experience = updateActivityDto.Experience ?? activity.Task.Experience;

        var result = await _context.SaveChangesAsync() > 0;

        if (result) return Ok();

        return BadRequest("Could not save changes to the database");
        
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteActivity(Guid id)
    {
        var activity = await _context.Activities.FindAsync(id);

        if(activity == null)
        {
            return NotFound();
        }

        _context.Activities.Remove(activity);
        var result = await _context.SaveChangesAsync() > 0;

        if (!result) return BadRequest("Could not save changes to the database");

        return Ok();
    }
}