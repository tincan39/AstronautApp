using StargateAPI.Business.Data;
using StargateAPI.Business.Queries;

namespace StargateTestBed.Business.Queries
{
    public class GetPeopleTests : BaseTester
    {
        private Task<GetPeopleResult> SendAsync() =>
            new GetPeopleHandler(Context).Handle(new GetPeople(), CancellationToken.None);

        [Test]
        public async Task Handle_PeopleExist_ReturnsEveryPerson()
        {
            var civilianName = "Neil Armstrong";
            var astronautName = "Sally Ride";
            await SeedPersonAsync(civilianName);
            await SeedPersonAsync(astronautName, new AstronautDetail
            {
                CurrentRank = "CPT",
                CurrentDutyTitle = "Commander",
                CareerStartDate = new DateTime(2020, 1, 15)
            });

            var result = await SendAsync();

            Assert.That(result.People.Select(x => x.Name), Is.EquivalentTo(new[] { civilianName, astronautName }));
        }

        [Test]
        public async Task Handle_NoPeople_ReturnsEmptyList()
        {
            var result = await SendAsync();

            Assert.That(result.People, Is.Empty);
        }
    }
}
