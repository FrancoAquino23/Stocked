namespace Stocked.Api.DTOs;

// DTO: Petición para guardar el día/hora de despensa
public class UpdatePantryScheduleRequestDto
{
    public DayOfWeek DayOfWeek { get; set; }
    public TimeOnly Time { get; set; }
}
