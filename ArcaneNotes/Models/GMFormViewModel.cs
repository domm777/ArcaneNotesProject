using EntityLibrary.Entities;

namespace ArcaneNotes.Models;

public class GMFormViewModel
{
    public WorkSpaceDTO? currentWorkSpace;
    public Collaberator selectedNote;
    public string userId;
    public GMFormViewModel()
    {
    }

    public GMFormViewModel(WorkSpaceDTO space, Collaberator startinguser)
    {
        selectedNote = startinguser;
        currentWorkSpace = space;
    }
}