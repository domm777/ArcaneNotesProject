using EntityLibrary.Entities;

namespace ArcaneNotes.Models;

public class GMFormViewModel
{
    public WorkSpace? currentWorkSpace;
    public string selectedNoteId;
    public GMFormViewModel()
    {
    }

    public GMFormViewModel(WorkSpace space, string startingId)
    {
        selectedNoteId = startingId;
        currentWorkSpace = space;
    }
}