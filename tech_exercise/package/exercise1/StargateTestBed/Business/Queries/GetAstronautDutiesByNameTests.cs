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
            var sally = await SeedPersonAsync("Sally Ride");
            var neil = await SeedPersonAsync("Neil Armstrong");
            await SeedDutyAsync(sally, "1LT", "Pilot", new DateTime(2020, 1, 15), new DateTime(2022, 5, 31));
            await SeedDutyAsync(sally, "CPT", "Commander", new DateTime(2022, 6, 1));
            await SeedDutyAsync(neil, "MAJ", "Engineer", new DateTime(2020, 1, 15));

            var result = await SendAsync("Sally Ride");

            Assert.That(result.AstronautDuties.Select(x => x.DutyTitle), Is.EquivalentTo(new[] { "Pilot", "Commander" }));
        }

        [Test]
        public async Task Handle_UnknownName_ReturnsEmptyList()
        {
            var result = await SendAsync("Nobody");

            Assert.That(result.AstronautDuties, Is.Empty);
        }
    }
}
