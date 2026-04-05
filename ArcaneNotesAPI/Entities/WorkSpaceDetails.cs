using System.ComponentModel.DataAnnotations;

namespace ArcaneNotesAPI.Entities;

public class WorkSpaceDetails
{
    [Required]
    public string Title { get; set; }
    [Required]
    public string GMName { get; set; }
}