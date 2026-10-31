using Dapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using StargateAPI.Business.Data;
using StargateAPI.Business.Dtos;
using StargateAPI.Controllers;

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
            var result = new GetPeopleResult();
            var people = await _context.People.AsNoTracking().Include(x => x.AstronautDetail).
                Select(x => PersonDTO.FromPerson(x)).ToListAsync(cancellationToken);
            return new GetPeopleResult(people) ;
        }
    }

    public class GetPeopleResult : BaseResponse
    {
        public GetPeopleResult() { }

        public GetPeopleResult(List<PersonDTO> people)
        {
            People = people;
        }

        public List<PersonDTO> People { get; set; } = new List<PersonDTO> { };

    }
}
