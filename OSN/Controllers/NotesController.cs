using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OSN.Application.Features.Notes.Commands.CreateNote;
using OSN.Application.Models.Requests.Notes;
using OSN.Application.Models.Responses.Notes;
using OSN.Application.TokenConstants;
using OSN.Swagger.Attributes;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;

namespace OSN.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class NotesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IHttpContextAccessor _contextAccessor;

    public NotesController(IMediator mediator, IHttpContextAccessor contextAccessor)
    {
        _mediator = mediator;
        _contextAccessor = contextAccessor;
    }

    [HttpPost($"")]
    [SwaggerRequestType(typeof(CreateNoteRequest))]
    [SwaggerResponse(StatusCodes.Status201Created, "Note created successfully.", typeof(NoteResponse))]
    public async Task<NoteResponse> CreateNote([FromBody] CreateNoteRequest request)
    {
        var command = new CreateNoteCommand(request);

        return await _mediator.Send(command);
    }

    [HttpPut($"{{noteId}}")]
    [SwaggerRequestType(typeof(UpdateNoteRequest))]
    [SwaggerResponse(StatusCodes.Status200OK, "Note updated successfully.", typeof(NoteResponse))]
    public async Task<NoteResponse> UpdateNote([FromBody] UpdateNoteRequest request)
    {
        var command = new UpdateNoteCommand(request);

        return await _mediator.Send(command);
    }

    [HttpDelete($"{{noteId}}")]
    [SwaggerResponse(StatusCodes.Status204NoContent, "Note deleted successfully.")]
    public async Task<NoteResponse> DeleteNote(Guid noteId)
    {
        var command = new DeleteNoteCommand(noteId);

        return await _mediator.Send(command);
    }

    [HttpGet($"")]
    [SwaggerResponse(StatusCodes.Status200OK, "Notes retrieved successfully.", typeof(IEnumerable<NoteResponse>))]
    public async Task<IEnumerable<NoteResponse>> GetNotes()
    {
        var userId = _contextAccessor.HttpContext?.Items[TokenConstants.UserIdClaim] as Guid?;

        if (userId == null || userId == Guid.Empty)
            throw new UnauthorizedAccessException("User ID claim missing.");

        var query = new GetNotesQuery(userId.Value);
        return await _mediator.Send(query);
    }

    [HttpGet($"{{noteId}}")]
    [SwaggerResponse(StatusCodes.Status200OK, "Note retrieved successfully.", typeof(NoteResponse))]
    public async Task<NoteResponse> GetNote(Guid noteId)
    {
        var query = new GetNoteQuery(noteId);

        return await _mediator.Send(query);
    }
}