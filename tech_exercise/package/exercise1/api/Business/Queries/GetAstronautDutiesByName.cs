using Dapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using StargateAPI.Business.Data;
using StargateAPI.Business.Dtos;
using StargateAPI.Controllers;

namespace StargateAPI.Business.Queries
{
    public class GetAstronautDutiesByName : IRequest<GetAstronautDutiesByNameResult>
    {
        public string Name { get; set; } = string.Empty;
    }

    public class GetAstronautDutiesByNameHandler : IRequestHandler<GetAstronautDutiesByName, GetAstronautDutiesByNameResult>
    {
        private readonly StargateContext _context;

        public GetAstronautDutiesByNameHandler(StargateContext context)
        {
            _context = context;
        }

        public async Task<GetAstronautDutiesByNameResult> Handle(GetAstronautDutiesByName request, CancellationToken cancellationToken)
        {

            var result = new GetAstronautDutiesByNameResult();

            var duties = await _context.AstronautDuties.AsNoTracking().Include(x => x.Person).Where(x => x.Person.Name == request.Name).ToListAsync();

            var dutiesDTOs = duties.Select(d => AstronautDutyDTO.FromDuty(d)).ToList();

            return new GetAstronautDutiesByNameResult(request.Name, dutiesDTOs);

        }
    }

    public class GetAstronautDutiesByNameResult : BaseResponse
    {
        public string AstronautName { get; set; } = string.Empty;
        public List<AstronautDutyDTO> AstronautDuties { get; set; } = new List<AstronautDutyDTO>();

        public GetAstronautDutiesByNameResult() { }
        public GetAstronautDutiesByNameResult(string astronautName, List<AstronautDutyDTO> astronautDuties)
        {
            AstronautName = astronautName;
            AstronautDuties = astronautDuties;
        }
    }
}
