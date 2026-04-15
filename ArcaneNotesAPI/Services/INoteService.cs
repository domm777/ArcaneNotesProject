using System.Text.Json;
using System.Text.Json.Nodes;
using ArcaneNotesAPI.Entities;
using EntityLibrary.Entities;

namespace ArcaneNotesAPI.Services;

public interface INoteService
{
    Task<WorkSpace> CreateWorkspaceAsync(string ownerId, string title, string gmName);
    Task AddCollaboratorAsync(string workspaceId, string newEmail, string userId);
    
    Task<Note> SyncNoteAsync(string userId, string workspaceId, JsonElement note);
    Task<List<WorkSpace>> GetWorkspacesAsync(string userId);
    Task<Note?> GetUserNoteInWorkspaceAsync(string userId, string workspaceId, string myUserId);
    Task<WorkSpaceDTO> GetWorkSpaceAsync(string workSpace, string myUserId);
}