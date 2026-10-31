using Microsoft.EntityFrameworkCore;
using StargateAPI.Business.Commands;
using StargateAPI.Business.Data;
using StargateAPI.Business.Exceptions;
using System.Net;

namespace StargateTestBed.Business.Commands
{
    public class CreateAstronautDutyTests : BaseTester
    {
        private static readonly DateTime FirstStart = new(2020, 1, 15);
        private static readonly DateTime SecondStart = new(2022, 6, 1);

        private async Task SendAsync(CreateAstronautDuty request)
        {
            await new CreateAstronautDutyPreProcessor(Context).Process(request, CancellationToken.None);
            await new CreateAstronautDutyHandler(Context).Handle(request, CancellationToken.None);
        }

        private static CreateAstronautDuty Duty(string name, string rank, string title, DateTime start) =>
            new() { Name = name, Rank = rank, DutyTitle = title, DutyStartDate = start };

        private static void AssertClientException(ClientException? ex, HttpStatusCode status, string message)
        {
            using (Assert.EnterMultipleScope())
            {
                Assert.That(ex!.StatusCode, Is.EqualTo(status));
                Assert.That(ex.Message, Is.EqualTo(message));
            }
        }

        [Test]
        public async Task Send_FirstDuty_CreatesOpenDutyAndAstronautDetail()
        {
            var name = "Sally Ride";
            var rank = "1LT";
            var title = "Pilot";
            await SeedPersonAsync(name);

            await SendAsync(Duty(name, rank, title, FirstStart));

            using var db = CreateContext();
            var person = await db.People.Include(x => x.AstronautDetail).Include(x => x.AstronautDuties).SingleAsync();
            var duty = person.AstronautDuties.Single();
            using (Assert.EnterMultipleScope())
            {
                Assert.That(person.AstronautDetail, Is.Not.Null);
                Assert.That(duty.Rank, Is.EqualTo(rank));
                Assert.That(duty.DutyTitle, Is.EqualTo(title));
                Assert.That(duty.DutyStartDate, Is.EqualTo(FirstStart));
                Assert.That(duty.DutyEndDate, Is.Null);

                Assert.That(person.AstronautDetail!.CurrentRank, Is.EqualTo(rank));
                Assert.That(person.AstronautDetail.CurrentDutyTitle, Is.EqualTo(title));
                Assert.That(person.AstronautDetail.CareerStartDate, Is.EqualTo(FirstStart));
                Assert.That(person.AstronautDetail.CareerEndDate, Is.Null);
            }
        }

        [Test]
        public async Task Send_SecondDuty_ClosesPriorDutyAndUpdatesDetail()
        {
            var name = "Sally Ride";
            var priorTitle = "Pilot";
            var currentRank = "CPT";
            var currentTitle = "Commander";
            await SeedPersonAsync(name);
            await SendAsync(Duty(name, "1LT", priorTitle, FirstStart));

            await SendAsync(Duty(name, currentRank, currentTitle, SecondStart));

            using var db = CreateContext();
            var person = await db.People.Include(x => x.AstronautDetail).Include(x => x.AstronautDuties).SingleAsync();
            var prior = person.AstronautDuties.Single(x => x.DutyTitle == priorTitle);
            var current = person.AstronautDuties.Single(x => x.DutyTitle == currentTitle);
            using (Assert.EnterMultipleScope())
            {
                Assert.That(prior.DutyEndDate, Is.EqualTo(SecondStart.AddDays(-1)));
                Assert.That(current.DutyEndDate, Is.Null);

                Assert.That(person.AstronautDetail!.CurrentRank, Is.EqualTo(currentRank));
                Assert.That(person.AstronautDetail.CurrentDutyTitle, Is.EqualTo(currentTitle));
                Assert.That(person.AstronautDetail.CareerStartDate, Is.EqualTo(FirstStart));
            }
        }

        [Test]
        public async Task Send_RetiredDuty_SetsCareerEndDateToDayBeforeRetirement()
        {
            var name = "Sally Ride";
            var retired = "RETIRED";
            await SeedPersonAsync(name);
            await SendAsync(Duty(name, "1LT", "Pilot", FirstStart));

            await SendAsync(Duty(name, "CPT", retired, SecondStart));

            using var db = CreateContext();
            var detail = await db.AstronautDetails.SingleAsync();
            using (Assert.EnterMultipleScope())
            {
                Assert.That(detail.CurrentDutyTitle, Is.EqualTo(retired));
                Assert.That(detail.CareerEndDate, Is.EqualTo(SecondStart.AddDays(-1)));
            }
        }

        [Test]
        public async Task Send_UnknownPerson_ThrowsNotFound()
        {
            var ex = await Assert.ThrowsAsync<ClientException>(async () => await SendAsync(Duty("Nobody", "1LT", "Pilot", FirstStart)));

            AssertClientException(ex, HttpStatusCode.NotFound, "There is no person with this name");
        }

        [Test]
        public async Task Send_RetiredPerson_ThrowsBadRequest()
        {
            var name = "John Glenn";
            var rank = "COL";
            await SeedPersonAsync(name, new AstronautDetail
            {
                CurrentRank = rank,
                CurrentDutyTitle = "RETIRED",
                CareerStartDate = FirstStart,
                CareerEndDate = SecondStart
            });

            var ex = await Assert.ThrowsAsync<ClientException>(async () =>
                await SendAsync(Duty(name, rank, "Commander", SecondStart.AddYears(1))));

            AssertClientException(ex, HttpStatusCode.BadRequest, "Retired Astronauts cannot take on new duties");
        }

        [TestCase(0)]
        [TestCase(-30)]
        public async Task Send_StartNotAfterExistingDuty_ThrowsAndLeavesExistingDutyUnchanged(int daysFromExistingStart)
        {
            var name = "Sally Ride";
            var existingTitle = "Pilot";
            await SeedPersonAsync(name);
            await SendAsync(Duty(name, "1LT", existingTitle, SecondStart));

            var ex = await Assert.ThrowsAsync<ClientException>(async () =>
                await SendAsync(Duty(name, "CPT", "Commander", SecondStart.AddDays(daysFromExistingStart))));

            AssertClientException(ex, HttpStatusCode.BadRequest, "Astronaut duty must be after all prior duties");
            using var db = CreateContext();
            var duty = await db.AstronautDuties.SingleAsync();
            using (Assert.EnterMultipleScope())
            {
                Assert.That(duty.DutyTitle, Is.EqualTo(existingTitle));
                Assert.That(duty.DutyEndDate, Is.Null);
            }
        }

        [Test]
        public async Task Send_BlankRank_ThrowsBadRequest()
        {
            var name = "Sally Ride";
            await SeedPersonAsync(name);

            var ex = await Assert.ThrowsAsync<ClientException>(async () => await SendAsync(Duty(name, " ", "Pilot", FirstStart)));

            AssertClientException(ex, HttpStatusCode.BadRequest, "The rank field must be filled");
        }
    }
}
