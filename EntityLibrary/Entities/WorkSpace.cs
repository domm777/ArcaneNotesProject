using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace EntityLibrary.Entities;

public class WorkSpace
{
    public WorkSpace()
    {
    }

    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    
    public string Title { get; set; } = "Untitled";
    public string GMName { get; set; } = "Undefined";

    [BsonRepresentation(BsonType.ObjectId)]
    public string OwnerId { get; set; } = null!;
    
    [BsonRepresentation(BsonType.ObjectId)]
    public List<string> Collaborators { get; set; } = new();
}