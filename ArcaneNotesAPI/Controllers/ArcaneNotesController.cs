using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Nodes;
using ArcaneNotesAPI.Entities;
using ArcaneNotesAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/workspace")]
public class ArcaneNotesController : ControllerBase
{
    private readonly INoteService _noteService;

    public ArcaneNotesController(INoteService noteService)
    {
        _noteService = noteService;
    }

    [Authorize]
    // Create a new workspace
    [HttpPost]
    public async Task<IActionResult> CreateWorkspace([FromBody] WorkSpaceDetails details)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId)) return Unauthorized();
        var workspace = await _noteService.CreateWorkspaceAsync(userId, details.Title, details.GMName);
        return Ok(workspace);
    }

    [Authorize]
    // Add a collaborator to a workspace
    [HttpPost("{workspaceId}/{newEmail}/collaborator")]
    public async Task<IActionResult> AddCollaborator(
        string workspaceId, 
        string newEmail)
    {
        try 
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();
            await _noteService.AddCollaboratorAsync(workspaceId, newEmail, userId);
            return Ok();
        }
        catch (UnauthorizedAccessException) { return Forbid(); }
        catch (Exception ex) { return BadRequest(ex.Message); }
    }
    
    [Authorize]
    //adds a note or updates a note in the workspace
    [HttpPut("{workspaceId}/note")]
    public async Task<IActionResult> SyncNote(
        string workspaceId, 
        [FromBody] JsonElement payload)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();
            JsonElement dataToSave = payload;
            if (payload.ValueKind == JsonValueKind.Object && payload.TryGetProperty("NoteData", out var innerData))
            {
                dataToSave = innerData;
            }
            var savedNote = await _noteService.SyncNoteAsync(userId, workspaceId, dataToSave);
            return Ok(savedNote);
        }
        catch (UnauthorizedAccessException) { return Forbid(); }
        catch (Exception ex) { return BadRequest(ex.Message); }
    }

    [Authorize]
    //Get all workspaces so the user can see which they are in
    [HttpGet]
    public async Task<IActionResult> GetWorkSpaces()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId)) return Unauthorized();
        var workspaces = await _noteService.GetWorkspacesAsync(userId);
        return Ok(workspaces);
    }
    
    [Authorize]
    //Get a specific workspace
    [HttpGet("{workspaceId}")]
    public async Task<IActionResult> GetWorkSpace(string workspaceId)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId)) return Unauthorized();
        var workspace = await _noteService.GetWorkSpaceAsync(workspaceId, userId);
        return Ok(workspace);
    }

    [Authorize]
    [HttpGet("{workspaceId}/{userId}/note")]
    public async Task<IActionResult> GetNote(string workspaceId, string userId)
    {
        var myUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(myUserId)) return Unauthorized();
        var note = await _noteService.GetUserNoteInWorkspaceAsync(userId, workspaceId, myUserId);
        if (note == null) return NotFound();
        return Ok(note);
    }
}