using EntityLibrary.Entities;

namespace ArcaneNotes.Models;

public class GMFormViewModel
{
    public WorkSpaceDTO? currentWorkSpace;
    public Collaberator selectedNote;
    public GMFormViewModel()
    {
    }

    public GMFormViewModel(WorkSpaceDTO space, Collaberator startinguser)
    {
        selectedNote = startinguser;
        currentWorkSpace = space;
    }
}