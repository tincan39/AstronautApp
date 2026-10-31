using MediatR;
using Microsoft.AspNetCore.Mvc;
using StargateAPI.Business.Commands;
using StargateAPI.Business.Queries;
using System.Net;

namespace StargateAPI.Controllers
{
   
    [ApiController]
    [Route("[controller]")]
    public class PersonController : ControllerBase
    {
        private readonly IMediator _mediator;
        public PersonController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("")]
        public async Task<IActionResult> GetPeople(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetPeople(), cancellationToken);

            return Ok(result);
        }

        [HttpGet("{name}")]
        public async Task<IActionResult> GetPersonByName(string name, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetPersonByName()
            {
                Name = name
            }, cancellationToken);

            return Ok(result);
        }

        [HttpPost("")]
        public async Task<IActionResult> CreatePerson([FromBody] string name, CancellationToken cancellationToken)
        {
            await _mediator.Send(new CreatePerson()
            {
                Name = name
            }, cancellationToken);
            return Ok();
        }
    }
}