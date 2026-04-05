using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ArcaneNotesAPI.Entities;

public class User
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    
    [JsonIgnore] 
    public string PasswordHash { get; set; } = null!;
}