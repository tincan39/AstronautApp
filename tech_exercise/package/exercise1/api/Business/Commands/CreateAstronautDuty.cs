using MediatR;
using MediatR.Pipeline;
using Microsoft.EntityFrameworkCore;
using StargateAPI.Business.Data;
using StargateAPI.Controllers;

namespace StargateAPI.Business.Commands
{
    public class CreateAstronautDuty : IRequest<CreateAstronautDutyResult>
    {
        public required string Name { get; set; }

        public required string Rank { get; set; }

        public required string DutyTitle { get; set; }

        public DateTime DutyStartDate { get; set; }
    }

    public class CreateAstronautDutyPreProcessor : IRequestPreProcessor<CreateAstronautDuty>
    {
        private readonly StargateContext _context;

        public CreateAstronautDutyPreProcessor(StargateContext context)
        {
            _context = context;
        }

        public async Task Process(CreateAstronautDuty request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Name)) throw new BadHttpRequestException("A person's name must be provided");
            if (string.IsNullOrWhiteSpace(request.Rank)) throw new BadHttpRequestException("The rank field must be filled");
            if (string.IsNullOrWhiteSpace(request.DutyTitle)) throw new BadHttpRequestException("The duty title field must be filled");

            if (request.DutyStartDate == default) throw new BadHttpRequestException("A vaild start date must be provided");

            var person = await _context.People.AsNoTracking().Include(x => x.AstronautDetail).FirstOrDefaultAsync(z => z.Name == request.Name, cancellationToken);

            if (person is null) throw new BadHttpRequestException("There is no person with this name");
            if (person.IsRetired) throw new BadHttpRequestException("Retired Astronauts cannot take on new duties");

            var conflictingDuties = await _context.AstronautDuties.Include(x => x.Person).Where(x => request.DutyStartDate.Date <= x.DutyStartDate.Date && x.Person.Name == request.Name).ToListAsync(cancellationToken);
            if (conflictingDuties.Count != 0) throw new BadHttpRequestException("Astronaut duty must be after all prior duties");
        }
    }

    public class CreateAstronautDutyHandler : IRequestHandler<CreateAstronautDuty, CreateAstronautDutyResult>
    {
        private readonly StargateContext _context;

        public CreateAstronautDutyHandler(StargateContext context)
        {
            _context = context;
        }
        public async Task<CreateAstronautDutyResult> Handle(CreateAstronautDuty request, CancellationToken cancellationToken)
        {
            var person = await _context.People.Include(x => x.AstronautDetail).FirstOrDefaultAsync(x => x.Name == request.Name, cancellationToken);

            var newAstronautDuty = new AstronautDuty()
            {
                PersonId = person.Id,
                Rank = request.Rank,
                DutyTitle = request.DutyTitle,
                DutyStartDate = request.DutyStartDate.Date,
                DutyEndDate = null
            };

            person.CreateOrUpdateAstronautDetails(newAstronautDuty);

            var priorDuty = await _context.AstronautDuties.Where(x => x.PersonId == person.Id && x.DutyEndDate == null).FirstOrDefaultAsync(cancellationToken);

            if (priorDuty != null)
                priorDuty.DutyEndDate = request.DutyStartDate.Date.AddDays(-1);

            await _context.AstronautDuties.AddAsync(newAstronautDuty, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);

            return new CreateAstronautDutyResult()
            {
                Id = newAstronautDuty.Id
            };
        }
    }

    public class CreateAstronautDutyResult : BaseResponse
    {
        public int? Id { get; set; }
    }
}
