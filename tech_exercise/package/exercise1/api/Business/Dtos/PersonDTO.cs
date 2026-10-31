using StargateAPI.Business.Data;

namespace StargateAPI.Business.Dtos
{
    public class PersonDTO
    {
        public string Name { get; set; } = string.Empty;

        public string CurrentRank { get; set; } = string.Empty;

        public string CurrentDutyTitle { get; set; } = string.Empty;

        public DateTime? CareerStartDate { get; set; }

        public DateTime? CareerEndDate { get; set; }

        // Requires person.AstronautDetail to be loaded (e.g. via Include).
        public static PersonDTO FromPerson(Person person)
        {
            return new PersonDTO
            {
                Name = person.Name,
                CurrentRank = person.AstronautDetail?.CurrentRank ?? string.Empty,
                CurrentDutyTitle = person.AstronautDetail?.CurrentDutyTitle ?? string.Empty,
                CareerStartDate = person.AstronautDetail?.CareerStartDate,
                CareerEndDate = person.AstronautDetail?.CareerEndDate
            };
        }
    }
}
