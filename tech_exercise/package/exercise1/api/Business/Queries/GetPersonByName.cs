using Dapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using StargateAPI.Business.Data;
using StargateAPI.Business.Dtos;
using StargateAPI.Business.Exceptions;
using StargateAPI.Controllers;
using System.Net;

namespace StargateAPI.Business.Queries
{
    public class GetPersonByName : IRequest<GetPersonByNameResult>
    {
        public required string Name { get; set; } = string.Empty;
    }

    public class GetPersonByNameHandler : IRequestHandler<GetPersonByName, GetPersonByNameResult>
    {
        private readonly StargateContext _context;
        public GetPersonByNameHandler(StargateContext context)
        {
            _context = context;
        }

        public async Task<GetPersonByNameResult> Handle(GetPersonByName request, CancellationToken cancellationToken)
        {
            var person = await _context.People.AsNoTracking().Include(x => x.AstronautDetail).FirstOrDefaultAsync(x => x.Name == request.Name, cancellationToken);
            if (person is null)
                throw new ClientException(HttpStatusCode.NotFound, "There is no Person with this name");

            return new GetPersonByNameResult(PersonDTO.FromPerson(person));
        }
    }

    public class GetPersonByNameResult
    {
        public PersonDTO? Person { get; set; }

        public GetPersonByNameResult(PersonDTO person)
        {
            this.Person = person;
        }
    }
}
