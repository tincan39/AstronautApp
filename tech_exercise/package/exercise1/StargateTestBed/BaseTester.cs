using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using StargateAPI.Business.Data;

namespace StargateTestBed
{
    public abstract class BaseTester
    {
        private SqliteConnection _connection = null!;

        protected StargateContext Context { get; private set; } = null!;

        [SetUp]
        public virtual void Setup()
        {
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            Context = CreateContext();
            Context.Database.EnsureCreated();
        }

        [TearDown]
        public virtual void TearDown()
        {
            Context.Dispose();
            _connection.Dispose();
        }

        protected StargateContext CreateContext() =>
            new(new DbContextOptionsBuilder<StargateContext>().UseSqlite(_connection).Options);

        protected async Task<Person> SeedPersonAsync(string name, AstronautDetail? detail = null)
        {
            var person = new Person { Name = name, AstronautDetail = detail };
            Context.People.Add(person);
            await Context.SaveChangesAsync();
            return person;
        }

        protected async Task<AstronautDuty> SeedDutyAsync(Person person, string rank, string dutyTitle, DateTime start, DateTime? end = null)
        {
            var duty = new AstronautDuty
            {
                PersonId = person.Id,
                Rank = rank,
                DutyTitle = dutyTitle,
                DutyStartDate = start,
                DutyEndDate = end
            };
            Context.AstronautDuties.Add(duty);
            await Context.SaveChangesAsync();
            return duty;
        }
    }
}
