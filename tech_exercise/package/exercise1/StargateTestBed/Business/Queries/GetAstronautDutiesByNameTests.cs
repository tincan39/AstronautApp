using StargateAPI.Business.Queries;

namespace StargateTestBed.Business.Queries
{
    public class GetAstronautDutiesByNameTests : BaseTester
    {
        private Task<GetAstronautDutiesByNameResult> SendAsync(string name) =>
            new GetAstronautDutiesByNameHandler(Context).Handle(new GetAstronautDutiesByName { Name = name }, CancellationToken.None);

        [Test]
        public async Task Handle_PersonWithDuties_ReturnsOnlyThatPersonsDuties()
        {
            var name = "Sally Ride";
            var pilotTitle = "Pilot";
            var commanderTitle = "Commander";
            var careerStart = new DateTime(2020, 1, 15);
            var commanderStart = new DateTime(2022, 6, 1);
            var sally = await SeedPersonAsync(name);
            var neil = await SeedPersonAsync("Neil Armstrong");
            await SeedDutyAsync(sally, "1LT", pilotTitle, careerStart, commanderStart.AddDays(-1));
            await SeedDutyAsync(sally, "CPT", commanderTitle, commanderStart);
            await SeedDutyAsync(neil, "MAJ", "Engineer", careerStart);

            var result = await SendAsync(name);

            Assert.That(result.AstronautDuties.Select(x => x.DutyTitle), Is.EquivalentTo(new[] { pilotTitle, commanderTitle }));
        }

        [Test]
        public async Task Handle_UnknownName_ReturnsEmptyList()
        {
            var result = await SendAsync("Nobody");

            Assert.That(result.AstronautDuties, Is.Empty);
        }
    }
}
