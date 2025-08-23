using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OSN.Application.Features.Notes;
using OSN.Application.Features.Notes.Create;
using OSN.Application.Features.Notes.Delete;
using OSN.Application.Features.Notes.Get;
using OSN.Application.Features.Notes.GetAll;
using OSN.Application.Features.Notes.Update;
using OSN.Domain.Core;

namespace OSN.Controllers;

[ApiController]
[Authorize(Policy = RoleHierarchy.GuestPolicy)]
[Route("api/[controller]")]
public class NoteController: ControllerBase
{
    private readonly IMediator _mediator;
    public NoteController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<List<NoteResponse>>> GetAll()
    {
        var command = new GetAllNotesQuery();
        var result = await _mediator.Send(command);
        if(!result.IsSuccess)
            return BadRequest(new { message = result.Error });
        return Ok(result.Value);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<NoteResponse>> GetById(GetNoteByIdQuery command)
    {
        var result = await _mediator.Send(command);
        if (!result.IsSuccess)
            return BadRequest(new { message = result.Error });
        return Ok(result.Value);
    }

    [HttpPost]
    public async Task<ActionResult<NoteResponse>> Create(CreateNoteCommand command)
    {
        var result = await _mediator.Send(command);
        if (!result.IsSuccess)
            return BadRequest(new { message = result.Error });
        return Ok(result.Value);
    }

    [HttpPut]
    public async Task<ActionResult<NoteResponse>> Update(UpdateNoteCommand command)
    {
        var result = await _mediator.Send(command);
        if (!result.IsSuccess)
            return BadRequest(new { message = result.Error });
        return Ok(result.Value);
    }

    [HttpDelete]
    public async Task<IActionResult> Delete(DeleteNoteCommand command)
    {
        var result = await _mediator.Send(command);
        if (!result.IsSuccess)
            return BadRequest(new { message = result.Error });
        return NoContent();
    }
}