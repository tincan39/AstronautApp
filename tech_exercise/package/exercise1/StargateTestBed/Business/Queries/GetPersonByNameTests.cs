using StargateAPI.Business.Data;
using StargateAPI.Business.Exceptions;
using StargateAPI.Business.Queries;
using System.Net;

namespace StargateTestBed.Business.Queries
{
    public class GetPersonByNameTests : BaseTester
    {
        private Task<GetPersonByNameResult> SendAsync(string name) =>
            new GetPersonByNameHandler(Context).Handle(new GetPersonByName { Name = name }, CancellationToken.None);

        [Test]
        public async Task Handle_ExistingName_ReturnsPerson()
        {
            var name = "Neil Armstrong";
            await SeedPersonAsync(name);

            var result = await SendAsync(name);

            Assert.That(result.Person!.Name, Is.EqualTo(name));
        }

        [Test]
        public async Task Handle_AstronautLookedUpWithDifferentCase_ReturnsAstronautDetail()
        {
            var rank = "CPT";
            await SeedPersonAsync("Sally Ride", new AstronautDetail
            {
                CurrentRank = rank,
                CurrentDutyTitle = "Commander",
                CareerStartDate = new DateTime(2020, 1, 15)
            });

            var result = await SendAsync("sally ride");

            Assert.That(result.Person!.CurrentRank, Is.EqualTo(rank));
        }

        [Test]
        public async Task Handle_UnknownName_ThrowsNotFound()
        {
            var ex = await Assert.ThrowsAsync<ClientException>(async () => await SendAsync("Nobody"));

            using (Assert.EnterMultipleScope())
            {
                Assert.That(ex!.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
                Assert.That(ex.Message, Is.EqualTo("There is no Person with this name"));
            }
        }
    }
}
