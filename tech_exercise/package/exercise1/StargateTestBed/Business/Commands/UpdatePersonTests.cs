using Microsoft.EntityFrameworkCore;
using StargateAPI.Business.Commands;
using StargateAPI.Business.Exceptions;
using System.Net;

namespace StargateTestBed.Business.Commands
{
    public class UpdatePersonTests : BaseTester
    {
        private async Task SendAsync(UpdatePerson request)
        {
            await new UpdatePersonPreProcessor(Context).Process(request, CancellationToken.None);
            await new UpdatePersonHandler(Context).Handle(request, CancellationToken.None);
        }

        private async Task<string[]> GetNamesAsync()
        {
            using var db = CreateContext();
            return await db.People.Select(x => x.Name).ToArrayAsync();
        }

        [Test]
        public async Task Send_NewName_RenamesPersonAndKeepsDuties()
        {
            var person = await SeedPersonAsync("Sally Ride");
            await SeedDutyAsync(person, "1LT", "Pilot", new DateTime(2020, 1, 15));

            await SendAsync(new UpdatePerson { Name = "Sally Ride", NewName = "Sally K. Ride" });

            using var db = CreateContext();
            var renamed = await db.People.Include(x => x.AstronautDuties).SingleAsync();
            using (Assert.EnterMultipleScope())
            {
                Assert.That(renamed.Name, Is.EqualTo("Sally K. Ride"));
                Assert.That(renamed.AstronautDuties.Select(x => x.DutyTitle), Is.EquivalentTo(new[] { "Pilot" }));
            }
        }

        [Test]
        public async Task Send_SameNameDifferentCase_RenamesPerson()
        {
            await SeedPersonAsync("john doe");

            await SendAsync(new UpdatePerson { Name = "john doe", NewName = "John Doe" });

            Assert.That(await GetNamesAsync(), Is.EqualTo(new[] { "John Doe" }));
        }

        [TestCase("")]
        [TestCase("   ")]
        public async Task Send_BlankNewName_ThrowsBadRequest(string newName)
        {
            await SeedPersonAsync("Sally Ride");

            var ex = await Assert.ThrowsAsync<ClientException>(async () =>
                await SendAsync(new UpdatePerson { Name = "Sally Ride", NewName = newName }));

            var names = await GetNamesAsync();
            using (Assert.EnterMultipleScope())
            {
                Assert.That(ex!.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                Assert.That(ex.Message, Is.EqualTo("A valid name must be provided to update a person"));
                Assert.That(names, Is.EqualTo(new[] { "Sally Ride" }));
            }
        }

        [Test]
        public async Task Send_UnknownPerson_ThrowsNotFound()
        {
            var ex = await Assert.ThrowsAsync<ClientException>(async () =>
                await SendAsync(new UpdatePerson { Name = "Nobody", NewName = "Somebody" }));

            using (Assert.EnterMultipleScope())
            {
                Assert.That(ex!.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
                Assert.That(ex.Message, Is.EqualTo("There is no person with this name"));
            }
        }

        [Test]
        public async Task Send_NewNameTakenByAnotherPerson_ThrowsBadRequest()
        {
            await SeedPersonAsync("Sally Ride");
            await SeedPersonAsync("Neil Armstrong");

            var ex = await Assert.ThrowsAsync<ClientException>(async () =>
                await SendAsync(new UpdatePerson { Name = "Sally Ride", NewName = "neil armstrong" }));

            var names = await GetNamesAsync();
            using (Assert.EnterMultipleScope())
            {
                Assert.That(ex!.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                Assert.That(ex.Message, Is.EqualTo("A person with this name already exists"));
                Assert.That(names, Is.EquivalentTo(new[] { "Sally Ride", "Neil Armstrong" }));
            }
        }
    }
}
