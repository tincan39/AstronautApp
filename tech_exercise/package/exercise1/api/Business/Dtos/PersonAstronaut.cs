using StargateAPI.Business.Data;

namespace StargateAPI.Business.Dtos
{
    public class PersonAstronaut
    {
        public int PersonId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string CurrentRank { get; set; } = string.Empty;

        public string CurrentDutyTitle { get; set; } = string.Empty;

        public DateTime? CareerStartDate { get; set; }

        public DateTime? CareerEndDate { get; set; }

        // Requires person.AstronautDetail to be loaded (e.g. via Include).
        public static PersonAstronaut FromPerson(Person person)
        {
            return new PersonAstronaut
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
