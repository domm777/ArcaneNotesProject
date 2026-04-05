using System.Text.Json;
using System.Text.Json.Nodes;
using ArcaneNotesAPI.Entities;
using EntityLibrary.Entities;
using MongoDB.Bson;
using MongoDB.Driver;

namespace ArcaneNotesAPI.Services;

public class NoteService : INoteService
{
    private readonly IMongoCollection<WorkSpace> _workspaces;
    private readonly IMongoCollection<Note> _notes;
    private readonly IMongoCollection<User> _users;

    public NoteService(IMongoClient client)
    {
        var database = client.GetDatabase("ArcaneNotesDB");
        _workspaces = database.GetCollection<WorkSpace>("Workspaces");
        _notes = database.GetCollection<Note>("Notes");
        _users = database.GetCollection<User>("Users");
    }

    public async Task<WorkSpace> CreateWorkspaceAsync(string ownerId, string title, string gmName)
    {
        var newWorkspace = new WorkSpace
        {
            OwnerId = ownerId,
            Title = title,
            GMName = gmName,
            Collaborators = new List<string>()
        };

        await _workspaces.InsertOneAsync(newWorkspace);

        var emptyPayload = JsonDocument.Parse("{}").RootElement;
        await UpsertNoteInternalAsync(ownerId, newWorkspace.Id, emptyPayload);

        return newWorkspace;
    }

    public async Task<Note> SyncNoteAsync(string userId, string workspaceId, JsonElement payload)
    {
        // 1. Enforce Auth
        var workSpaceAuth = await _workspaces.Find(w => w.Id == workspaceId)
            .Project(w => new { w.OwnerId, w.Collaborators })
            .FirstOrDefaultAsync();

        if (workSpaceAuth == null ||
            (workSpaceAuth.OwnerId != userId && !workSpaceAuth.Collaborators.Contains(userId)))
        {
            throw new UnauthorizedAccessException("Access denied.");
        }
        
        return await UpsertNoteInternalAsync(userId, workspaceId, payload);
    }
    
    private async Task<Note> UpsertNoteInternalAsync(string userId, string workspaceId, JsonElement payload)
    {
        var filter = Builders<Note>.Filter.And(
            Builders<Note>.Filter.Eq(n => n.WorkspaceId, workspaceId),
            Builders<Note>.Filter.Eq(n => n.OwnerId, userId)
        );
    
        // Serialize the payload to a string to match the DB-mapped property
        string rawPayload = JsonSerializer.Serialize(payload);

        var update = Builders<Note>.Update
            .Set(n => n.OwnerId, userId)
            .Set(n => n.WorkspaceId, workspaceId)
            .Set(n => n.RawNoteData, rawPayload); // Target RawNoteData, not NoteData

        var options = new FindOneAndUpdateOptions<Note>
        {
            IsUpsert = true,
            ReturnDocument = ReturnDocument.After
        };

        return await _notes.FindOneAndUpdateAsync(filter, update, options);
    }

    public async Task AddCollaboratorAsync(string workspaceId, string newEmail, string myUserId)
    {
        string normalizedEmail = newEmail.Trim().ToLowerInvariant();
        
        var userFilter = Builders<User>.Filter.Eq(u => u.Email, normalizedEmail);
        var targetUserId = await _users
            .Find(userFilter)
            .Project(u => u.Id) 
            .FirstOrDefaultAsync();
        
        if (targetUserId == null)
        {
            throw new ArgumentException("User with this email does not exist.");
        }
        
        if (targetUserId == myUserId)
        {
            throw new InvalidOperationException("Owner cannot be added as a collaborator.");
        }

        var workspaceFilter = Builders<WorkSpace>.Filter.And(
            Builders<WorkSpace>.Filter.Eq(w => w.Id, workspaceId),
            Builders<WorkSpace>.Filter.Eq(w => w.OwnerId, myUserId)
        );
        
        var update = Builders<WorkSpace>.Update.AddToSet(w => w.Collaborators, targetUserId);
        var result = await _workspaces.UpdateOneAsync(workspaceFilter, update);

        if (result.MatchedCount == 0) 
        {
            throw new UnauthorizedAccessException("WorkSpace not found or unauthorized.");
        }
        
        if (result.ModifiedCount == 0)
        {
            throw new InvalidOperationException("User is already a collaborator."); 
        }
        //if everything passed create new note for the user in the db
        var emptyPayload = JsonDocument.Parse("{}").RootElement;
        await UpsertNoteInternalAsync(targetUserId, workspaceId, emptyPayload);
    }
    public async Task<List<WorkSpace>> GetWorkspacesAsync(string userId)
    {
        var filter = Builders<WorkSpace>.Filter.Where(w => 
            w.OwnerId == userId || w.Collaborators.Contains(userId));
        List<WorkSpace> workspaces = await _workspaces.Find(filter).ToListAsync();
        return workspaces;
    }

    public async Task<List<Note>> GetWorkspaceNotesAsync(string userId, string workspaceId)
    {
        // Simple security check before returning all notes in a room
        var WorkSpace = await _workspaces.Find(w => w.Id == workspaceId).FirstOrDefaultAsync();
        if (WorkSpace == null || (WorkSpace.OwnerId != userId && !WorkSpace.Collaborators.Contains(userId)))
            return new List<Note>();
        return await _notes.Find(n => n.WorkspaceId == workspaceId).ToListAsync();
    }

    public async Task<Note?> GetUserNoteInWorkspaceAsync(string userId, string workspaceId, string myUserId)
    {
        var WorkSpace = await _workspaces.Find(w => w.Id == workspaceId).FirstOrDefaultAsync();
        if (WorkSpace == null || (WorkSpace.OwnerId != myUserId && !WorkSpace.Collaborators.Contains(myUserId)))
            return null;
        var note = await _notes.Find(n => n.WorkspaceId == workspaceId && n.OwnerId == userId).FirstOrDefaultAsync();
        if (myUserId != userId)
        {
            var rootNode = JsonNode.Parse(note.RawNoteData);
        
            if (rootNode is JsonArray cardsArray)
            {
                FilterHiddenCards(cardsArray);
                note.NoteData = JsonSerializer.Deserialize<JsonElement>(rootNode.ToJsonString());
            }
        }
        return note;
    }
    
    private void FilterHiddenCards(JsonArray cards)
    {
        for (int i = cards.Count - 1; i >= 0; i--)
        {
            var card = cards[i] as JsonObject;
            if (card == null) continue;
            
            if (card.TryGetPropertyValue("Hidden", out var hiddenNode) && 
                hiddenNode?.GetValue<bool>() == true)
            {
                cards.RemoveAt(i);
                continue;
            }
            
            if (card.TryGetPropertyValue("CardData", out var cardDataNode) && 
                cardDataNode is JsonObject cardDataObj)
            {
                if (cardDataObj.TryGetPropertyValue("Children", out var childrenNode) && 
                    childrenNode is JsonArray childrenArray)
                {
                    FilterHiddenCards(childrenArray);
                }
            }
        }
    }

    public async Task<WorkSpace> GetWorkSpaceAsync(string workSpace, string myUserId)
    {
        var WorkSpace = await _workspaces.Find(w => w.Id == workSpace).FirstOrDefaultAsync();
        if (WorkSpace == null || (WorkSpace.OwnerId != myUserId && !WorkSpace.Collaborators.Contains(myUserId)))
            return null;
        return WorkSpace;
    }
}