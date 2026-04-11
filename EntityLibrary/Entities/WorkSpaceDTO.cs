using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace EntityLibrary.Entities;

public class WorkSpaceDTO
{
    public WorkSpaceDTO()
    {
    }

    public WorkSpaceDTO(List<Collaberator> collabs, WorkSpace ws)
    {
        Collaborators = collabs;
        GMName = ws.GMName;
        Id = ws.Id;
        OwnerId = ws.OwnerId;
        Title = ws.Title;
    }

    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    public string Title { get; set; } = "Untitled";
    public string GMName { get; set; } = "Undefined";

    [BsonRepresentation(BsonType.ObjectId)]
    public string OwnerId { get; set; } = null!;
    
    [BsonRepresentation(BsonType.ObjectId)]
    public List<Collaberator> Collaborators { get; set; } = new();
}