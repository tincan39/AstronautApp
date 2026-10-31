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
            await SeedPersonAsync("Sally Ride");

            await SendAsync(Duty("Sally Ride", "1LT", "Pilot", FirstStart));

            using var db = CreateContext();
            var person = await db.People.Include(x => x.AstronautDetail).Include(x => x.AstronautDuties).SingleAsync();
            var duty = person.AstronautDuties.Single();
            using (Assert.EnterMultipleScope())
            {
                Assert.That(person.AstronautDetail, Is.Not.Null);
                Assert.That(duty.Rank, Is.EqualTo("1LT"));
                Assert.That(duty.DutyTitle, Is.EqualTo("Pilot"));
                Assert.That(duty.DutyStartDate, Is.EqualTo(FirstStart));
                Assert.That(duty.DutyEndDate, Is.Null);

                Assert.That(person.AstronautDetail!.CurrentRank, Is.EqualTo("1LT"));
                Assert.That(person.AstronautDetail.CurrentDutyTitle, Is.EqualTo("Pilot"));
                Assert.That(person.AstronautDetail.CareerStartDate, Is.EqualTo(FirstStart));
                Assert.That(person.AstronautDetail.CareerEndDate, Is.Null);
            }
        }

        [Test]
        public async Task Send_SecondDuty_ClosesPriorDutyAndUpdatesDetail()
        {
            await SeedPersonAsync("Sally Ride");
            await SendAsync(Duty("Sally Ride", "1LT", "Pilot", FirstStart));

            await SendAsync(Duty("Sally Ride", "CPT", "Commander", SecondStart));

            using var db = CreateContext();
            var person = await db.People.Include(x => x.AstronautDetail).Include(x => x.AstronautDuties).SingleAsync();
            var prior = person.AstronautDuties.Single(x => x.DutyTitle == "Pilot");
            var current = person.AstronautDuties.Single(x => x.DutyTitle == "Commander");
            using (Assert.EnterMultipleScope())
            {
                Assert.That(prior.DutyEndDate, Is.EqualTo(SecondStart.AddDays(-1)));
                Assert.That(current.DutyEndDate, Is.Null);

                Assert.That(person.AstronautDetail!.CurrentRank, Is.EqualTo("CPT"));
                Assert.That(person.AstronautDetail.CurrentDutyTitle, Is.EqualTo("Commander"));
                Assert.That(person.AstronautDetail.CareerStartDate, Is.EqualTo(FirstStart));
            }
        }

        [Test]
        public async Task Send_RetiredDuty_SetsCareerEndDateToDayBeforeRetirement()
        {
            await SeedPersonAsync("Sally Ride");
            await SendAsync(Duty("Sally Ride", "1LT", "Pilot", FirstStart));

            await SendAsync(Duty("Sally Ride", "CPT", "RETIRED", SecondStart));

            using var db = CreateContext();
            var detail = await db.AstronautDetails.SingleAsync();
            using (Assert.EnterMultipleScope())
            {
                Assert.That(detail.CurrentDutyTitle, Is.EqualTo("RETIRED"));
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
            await SeedPersonAsync("John Glenn", new AstronautDetail
            {
                CurrentRank = "COL",
                CurrentDutyTitle = "RETIRED",
                CareerStartDate = FirstStart,
                CareerEndDate = SecondStart
            });

            var ex = await Assert.ThrowsAsync<ClientException>(async () =>
                await SendAsync(Duty("John Glenn", "COL", "Commander", SecondStart.AddYears(1))));

            AssertClientException(ex, HttpStatusCode.BadRequest, "Retired Astronauts cannot take on new duties");
        }

        [TestCase(0)]
        [TestCase(-30)]
        public async Task Send_StartNotAfterExistingDuty_ThrowsAndLeavesExistingDutyUnchanged(int daysFromExistingStart)
        {
            await SeedPersonAsync("Sally Ride");
            await SendAsync(Duty("Sally Ride", "1LT", "Pilot", SecondStart));

            var ex = await Assert.ThrowsAsync<ClientException>(async () =>
                await SendAsync(Duty("Sally Ride", "CPT", "Commander", SecondStart.AddDays(daysFromExistingStart))));

            AssertClientException(ex, HttpStatusCode.BadRequest, "Astronaut duty must be after all prior duties");
            using var db = CreateContext();
            var duty = await db.AstronautDuties.SingleAsync();
            using (Assert.EnterMultipleScope())
            {
                Assert.That(duty.DutyTitle, Is.EqualTo("Pilot"));
                Assert.That(duty.DutyEndDate, Is.Null);
            }
        }

        [Test]
        public async Task Send_BlankRank_ThrowsBadRequest()
        {
            await SeedPersonAsync("Sally Ride");

            var ex = await Assert.ThrowsAsync<ClientException>(async () => await SendAsync(Duty("Sally Ride", " ", "Pilot", FirstStart)));

            AssertClientException(ex, HttpStatusCode.BadRequest, "The rank field must be filled");
        }
    }
}
