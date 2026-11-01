using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using StargateAPI.Business.Commands;
using StargateAPI.Business.Data;
using StargateAPI.Business.Exceptions;
using StargateAPI.PipelineBehaviors;

namespace StargateTestBed.PipelineBehaviors
{
    public class LoggingBehaviorTests : BaseTester
    {
        [Test]
        public async Task Handle_HandlerThrows_StoresErrorLogAndRethrows()
        {
            var message = "A person with this name already exists";
            var behavior = new LoggingBehavior<CreatePerson, CreatePersonResult>(
                NullLogger<LoggingBehavior<CreatePerson, CreatePersonResult>>.Instance, Context);
            var ex = await Assert.ThrowsAsync<ClientException>(async () =>
                await behavior.Handle(new CreatePerson { Name = "John Doe" }, () => throw new ClientException(message), CancellationToken.None));

            using var db = CreateContext();
            var log = await db.Logs.SingleAsync();
            using (Assert.EnterMultipleScope())
            {
                Assert.That(ex!.Message, Is.EqualTo(message));
                Assert.That(log.RequestName, Is.EqualTo(nameof(CreatePerson)));
                Assert.That(log.Status, Is.EqualTo(LogEntry.StatusError));
                Assert.That(log.ExceptionMessage, Is.EqualTo(message));
                Assert.That(log.StackTrace, Is.Not.Null);
            }
        }
    }
}
