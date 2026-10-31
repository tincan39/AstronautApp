using MediatR;
using MediatR.Pipeline;
using Microsoft.EntityFrameworkCore;
using StargateAPI.Business.Data;
using StargateAPI.Business.Exceptions;
using System.Net;

namespace StargateAPI.Business.Commands
{
    public class UpdatePerson : IRequest
    {
        public required string Name { get; set; }

        public required string NewName { get; set; }
    }

    public class UpdatePersonPreProcessor : IRequestPreProcessor<UpdatePerson>
    {
        private readonly StargateContext _context;

        public UpdatePersonPreProcessor(StargateContext context)
        {
            _context = context;
        }

        public async Task Process(UpdatePerson request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.NewName)) throw new ClientException("A valid name must be provided to update a person");

            var person = await _context.People.AsNoTracking().FirstOrDefaultAsync(x => x.Name == request.Name, cancellationToken);
            if (person is null) throw new ClientException(HttpStatusCode.NotFound, "There is no person with this name");

            var nameTaken = await _context.People.AsNoTracking().AnyAsync(x => x.Name == request.NewName && x.Id != person.Id, cancellationToken);
            if (nameTaken) throw new ClientException("A person with this name already exists");
        }
    }

    public class UpdatePersonHandler : IRequestHandler<UpdatePerson>
    {
        private readonly StargateContext _context;

        public UpdatePersonHandler(StargateContext context)
        {
            _context = context;
        }

        public async Task Handle(UpdatePerson request, CancellationToken cancellationToken)
        {
            var person = await _context.People.FirstAsync(x => x.Name == request.Name, cancellationToken);

            person.Name = request.NewName;

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
