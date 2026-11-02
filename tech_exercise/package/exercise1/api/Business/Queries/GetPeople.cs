using MediatR;
using Microsoft.EntityFrameworkCore;
using StargateAPI.Business.Data;
using StargateAPI.Business.Dtos;

namespace StargateAPI.Business.Queries
{
    public class GetPeople : IRequest<GetPeopleResult>
    {

    }

    public class GetPeopleHandler : IRequestHandler<GetPeople, GetPeopleResult>
    {
        public readonly StargateContext _context;
        public GetPeopleHandler(StargateContext context)
        {
            _context = context;
        }
        public async Task<GetPeopleResult> Handle(GetPeople request, CancellationToken cancellationToken)
        {
            var people = await _context.People.AsNoTracking().Include(x => x.AstronautDetail).
                Select(x => PersonDTO.FromPerson(x)).ToListAsync(cancellationToken);
            return new GetPeopleResult(people);
        }
    }

    public record GetPeopleResult(List<PersonDTO> People);
}
