using MediatR;
using MediatR.Pipeline;
using Microsoft.EntityFrameworkCore;
using StargateAPI.Business.Data;
using StargateAPI.Business.Exceptions;
using StargateAPI.Controllers;
using System.Net;

namespace StargateAPI.Business.Commands
{
    public class CreatePerson : IRequest
    {
        public required string Name { get; set; } = string.Empty;
    }

    public class CreatePersonPreProcessor : IRequestPreProcessor<CreatePerson>
    {
        private readonly StargateContext _context;
        public CreatePersonPreProcessor(StargateContext context)
        {
            _context = context;
        }
        public async Task Process(CreatePerson request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Name)) throw new ClientException("A valid name must be provided to create a person");

            var person = await _context.People.AsNoTracking().FirstOrDefaultAsync(z => z.Name == request.Name, cancellationToken);

            if (person is not null) throw new ClientException("A person with this name already exists");
        }
    }

    public class CreatePersonHandler : IRequestHandler<CreatePerson>
    {
        private readonly StargateContext _context;

        public CreatePersonHandler(StargateContext context)
        {
            _context = context;
        }
        public async Task Handle(CreatePerson request, CancellationToken cancellationToken)
        {
            var newPerson = new Person()
            {
                Name = request.Name
            };

            await _context.People.AddAsync(newPerson, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
