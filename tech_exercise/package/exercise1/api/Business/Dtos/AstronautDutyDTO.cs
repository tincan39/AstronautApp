using StargateAPI.Business.Data;

namespace StargateAPI.Business.Dtos
{
    public record AstronautDutyDTO(
        string Rank,
        string DutyTitle,
        DateTime DutyStartDate,
        DateTime? DutyEndDate)
    {
        public static AstronautDutyDTO FromDuty(AstronautDuty duty) => new(
            duty.Rank,
            duty.DutyTitle,
            duty.DutyStartDate,
            duty.DutyEndDate);
    }
}
