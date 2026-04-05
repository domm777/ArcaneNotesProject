using EntityLibrary.Entities;

namespace ArcaneNotes.Models;

public class HomeViewModel
{
    public HomeViewModel()
    {
    }

    public List<WorkSpace> MyWorkspaces { get; set; }
    public List<WorkSpace> Collaborations { get; set; }

    public string title { get; set; }
    public string gmName { get; set; }

    public HomeViewModel(List<WorkSpace> allWorkspaces, string currentUserId)
    {
        MyWorkspaces = allWorkspaces.Where(w => w.OwnerId == currentUserId).ToList();
        Collaborations = allWorkspaces.Where(w => w.OwnerId != currentUserId 
                                                  && w.Collaborators.Contains(currentUserId)).ToList();
    }
}