using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OSN.Application;
using OSN.Application.Features.Notes;
using OSN.Application.Features.Notes.Create;
using OSN.Application.Features.Notes.Delete;
using OSN.Application.Features.Notes.Get;
using OSN.Application.Features.Notes.Update;

namespace OSN;

[ApiController]
[Authorize(Policy = RoleHierarchy.UserPolicy)]
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
        var notes = await _mediator.Send(new GetAllNotesQuery());
        return Ok(notes.Value);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<NoteResponse>> GetById(Guid id)
    {
        var note = await _mediator.Send(new GetNoteByIdQuery(id));
        return Ok(note.Value);
    }

    [HttpPost]
    public async Task<ActionResult<NoteResponse>> Create(CreateNoteRequest request)
    {
        var note = await _mediator.Send(new CreateNoteCommand(request));
        return CreatedAtAction(nameof(GetById), new { id = note.Value.Id }, note.Value);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<NoteResponse>> Update(Guid id, UpdateNoteRequest request)
    {
        var note = await _mediator.Send(new UpdateNoteCommand(id, request));
        return Ok(note.Value);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _mediator.Send(new DeleteNoteCommand(id));
        return NoContent();
    }
}