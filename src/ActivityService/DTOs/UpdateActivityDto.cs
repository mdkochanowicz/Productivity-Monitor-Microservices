namespace ActivityService.DTOs;

public class UpdateActivityDto
{
    public string Name {get; set;}
    public string Description {get; set;}
    public int? PredictedTime {get; set;}
    public string Category {get; set;}
    public int? Experience {get; set;}
}