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
            var oldName = "Sally Ride";
            var newName = "Sally K. Ride";
            var dutyTitle = "Pilot";
            var person = await SeedPersonAsync(oldName);
            await SeedDutyAsync(person, "1LT", dutyTitle, new DateTime(2020, 1, 15));

            await SendAsync(new UpdatePerson { Name = oldName, NewName = newName });

            using var db = CreateContext();
            var renamed = await db.People.Include(x => x.AstronautDuties).SingleAsync();
            using (Assert.EnterMultipleScope())
            {
                Assert.That(renamed.Name, Is.EqualTo(newName));
                Assert.That(renamed.AstronautDuties.Select(x => x.DutyTitle), Is.EquivalentTo(new[] { dutyTitle }));
            }
        }

        [Test]
        public async Task Send_SameNameDifferentCase_RenamesPerson()
        {
            var storedName = "john doe";
            var renamedName = "John Doe";
            await SeedPersonAsync(storedName);

            await SendAsync(new UpdatePerson { Name = storedName, NewName = renamedName });

            Assert.That(await GetNamesAsync(), Is.EqualTo(new[] { renamedName }));
        }

        [Test]
        public async Task Send_BlankNewName_ThrowsBadRequest()
        {
            var name = "Sally Ride";
            await SeedPersonAsync(name);

            var ex = await Assert.ThrowsAsync<ClientException>(async () =>
                await SendAsync(new UpdatePerson { Name = name, NewName = " " }));

            var names = await GetNamesAsync();
            using (Assert.EnterMultipleScope())
            {
                Assert.That(ex!.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                Assert.That(ex.Message, Is.EqualTo("A valid name must be provided to update a person"));
                Assert.That(names, Is.EqualTo(new[] { name }));
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
            var existingName = "Sally Ride";
            var otherName = "Neil Armstrong";
            await SeedPersonAsync(existingName);
            await SeedPersonAsync(otherName);

            var ex = await Assert.ThrowsAsync<ClientException>(async () =>
                await SendAsync(new UpdatePerson { Name = existingName, NewName = otherName.ToLowerInvariant() }));

            var names = await GetNamesAsync();
            using (Assert.EnterMultipleScope())
            {
                Assert.That(ex!.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                Assert.That(ex.Message, Is.EqualTo("A person with this name already exists"));
                Assert.That(names, Is.EquivalentTo(new[] { existingName, otherName }));
            }
        }
    }
}
