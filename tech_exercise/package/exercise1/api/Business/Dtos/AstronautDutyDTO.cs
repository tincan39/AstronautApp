using StargateAPI.Business.Data;

namespace StargateAPI.Business.Dtos
{
    public class AstronautDutyDTO
    {
        public string Rank { get; set; } = string.Empty;

        public string DutyTitle { get; set; } = string.Empty;

        public DateTime DutyStartDate { get; set; }

        public DateTime? DutyEndDate { get; set; }

        // The name is passed in because duties loaded via Dapper don't populate duty.Person.
        public static AstronautDutyDTO FromDuty(AstronautDuty duty)
        {
            return new AstronautDutyDTO
            {
                Rank = duty.Rank,
                DutyTitle = duty.DutyTitle,
                DutyStartDate = duty.DutyStartDate,
                DutyEndDate = duty.DutyEndDate
            };
        }
    }
}
