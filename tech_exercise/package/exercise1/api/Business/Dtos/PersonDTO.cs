using StargateAPI.Business.Data;

namespace StargateAPI.Business.Dtos
{
    public record PersonDTO(
        string Name,
        string CurrentRank,
        string CurrentDutyTitle,
        DateTime? CareerStartDate,
        DateTime? CareerEndDate)
    {
        // Requires person.AstronautDetail to be loaded (e.g. via Include).
        public static PersonDTO FromPerson(Person person) => new(
            person.Name,
            person.AstronautDetail?.CurrentRank ?? string.Empty,
            person.AstronautDetail?.CurrentDutyTitle ?? string.Empty,
            person.AstronautDetail?.CareerStartDate,
            person.AstronautDetail?.CareerEndDate);
    }
}
