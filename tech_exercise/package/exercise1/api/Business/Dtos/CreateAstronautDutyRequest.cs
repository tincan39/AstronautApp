namespace StargateAPI.Business.Dtos
{
    public record CreateAstronautDutyRequest(string Name, string Rank, string DutyTitle, DateTime DutyStartDate);
}
