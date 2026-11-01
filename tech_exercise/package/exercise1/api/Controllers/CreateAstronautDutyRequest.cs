namespace StargateAPI.Controllers
{
    public record CreateAstronautDutyRequest(string Name, string Rank, string DutyTitle, DateTime DutyStartDate);
}
