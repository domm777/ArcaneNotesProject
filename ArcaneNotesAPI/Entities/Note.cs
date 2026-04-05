using System.Text.Json;
using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class Note
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    [BsonIgnoreIfDefault]
    public string Id { get; set; }
    
    [BsonRepresentation(BsonType.ObjectId)]
    public string WorkspaceId { get; set; } = null!;
    
    [BsonRepresentation(BsonType.ObjectId)]
    public string OwnerId { get; set; } = null!;
    
    public string Type { get; set; } = "gm";

    // Backing fields
    private string _rawNoteData = "[]";
    private JsonElement? _cachedNoteData = null;

    [JsonIgnore]
    public string RawNoteData 
    { 
        get => _rawNoteData; 
        set 
        {
            _rawNoteData = value;
            _cachedNoteData = null; // Invalidate the parsed cache if the DB updates the raw string
        }
    }

    [BsonIgnore]
    public JsonElement NoteData
    {
        get 
        {
            // Return the cached struct if we already parsed it
            if (_cachedNoteData.HasValue) return _cachedNoteData.Value;
            
            if (string.IsNullOrWhiteSpace(RawNoteData)) return default;
            
            using var doc = JsonDocument.Parse(RawNoteData);
            _cachedNoteData = doc.RootElement.Clone();
            
            return _cachedNoteData.Value;
        }
        set 
        {
            _cachedNoteData = value;
            _rawNoteData = JsonSerializer.Serialize(value);
        }
    }
}