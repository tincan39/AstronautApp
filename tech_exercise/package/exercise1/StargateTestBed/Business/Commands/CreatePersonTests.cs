using Microsoft.EntityFrameworkCore;
using StargateAPI.Business.Commands;
using StargateAPI.Business.Exceptions;
using System.Net;

namespace StargateTestBed.Business.Commands
{
    public class CreatePersonTests : BaseTester
    {
        private async Task<CreatePersonResult> SendAsync(CreatePerson request)
        {
            await new CreatePersonPreProcessor(Context).Process(request, CancellationToken.None);
            return await new CreatePersonHandler(Context).Handle(request, CancellationToken.None);
        }

        [Test]
        public async Task Send_NewName_CreatesPersonWithoutAstronautDetail()
        {
            var name = "Neil Armstrong";

            var result = await SendAsync(new CreatePerson { Name = name });

            using var db = CreateContext();
            var person = await db.People.Include(x => x.AstronautDetail).SingleAsync();
            using (Assert.EnterMultipleScope())
            {
                Assert.That(person.Name, Is.EqualTo(name));
                Assert.That(person.AstronautDetail, Is.Null);
                Assert.That(result.Person.Name, Is.EqualTo(name));
                Assert.That(result.Person.CurrentRank, Is.Empty);
            }
        }

        [Test]
        public async Task Process_NameMatchesExistingPersonIgnoringCase_ThrowsClientException()
        {
            await SeedPersonAsync("John Doe");
            var preProcessor = new CreatePersonPreProcessor(Context);

            var ex = await Assert.ThrowsAsync<ClientException>(async () =>
                await preProcessor.Process(new CreatePerson { Name = "john doe" }, CancellationToken.None));

            using (Assert.EnterMultipleScope())
            {
                Assert.That(ex!.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                Assert.That(ex.Message, Is.EqualTo("A person with this name already exists"));
            }
        }

        [Test]
        public async Task Send_BlankName_ThrowsClientExceptionAndCreatesNothing()
        {
            var ex = await Assert.ThrowsAsync<ClientException>(async () => await SendAsync(new CreatePerson { Name = " " }));

            using var db = CreateContext();
            var peopleCount = await db.People.CountAsync();
            using (Assert.EnterMultipleScope())
            {
                Assert.That(ex!.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                Assert.That(ex.Message, Is.EqualTo("A valid name must be provided to create a person"));
                Assert.That(peopleCount, Is.Zero);
            }
        }
    }
}
