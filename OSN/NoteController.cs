using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace OSN;

[ApiController]
[Route("api/note")]
public class NoteController: ControllerBase
{
    private readonly IMediator _mediator;
    public NoteController(IMediator mediator)
    {
        _mediator = mediator;
    }


}