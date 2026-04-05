namespace DnDSessionNotes.Models
{
    public class NotesPayload
    {
        public string Date { get; set; }
        public string Role { get; set; }
        public string GmName { get; set; }
        public string PlayerName { get; set; }
        public string CharacterName { get; set; }
        public string Title { get; set; }

        public List<Npc> Npcs { get; set; } = new();
        public List<Location> Locations { get; set; } = new();
        public List<Treasure> Treasures { get; set; } = new();
        public List<PlotEntry> Plots { get; set; } = new();
        public List<ExtraInfoEntry> Extras { get; set; } = new();
    }

    public class Npc
    {
        public string Name { get; set; }
        public string Location { get; set; }
        public string SpokeAbout { get; set; }
        public string Relationship { get; set; }
        public string? Image { get; set; }
    }

    public class Location
    {
        public string Text { get; set; }
        public string? Image { get; set; }
    }

    public class Treasure
    {
        public int Platinum { get; set; }
        public int Gold { get; set; }
        public int Silver { get; set; }
        public int Copper { get; set; }
        public List<string> Items { get; set; } = new();
        public string? Image { get; set; }
    }

    public class PlotEntry
    {
        public string Text { get; set; }
        public string? Image { get; set; }
    }

    public class ExtraInfoEntry
    {
        public string Text { get; set; }
        public string? Image { get; set; }
    }
}
