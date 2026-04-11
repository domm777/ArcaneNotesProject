namespace ArcaneNotes.Models
{
	public class PdfExportPayload
	{
		public string Date { get; set; } = "";
		public string Role { get; set; } = "GM";
		public string Title { get; set; } = "";
		public string Name { get; set; } = "";
		public string Type { get; set; } = "gm";
		public List<PdfCardData> NoteData { get; set; } = new();
	}

	public class PdfCardData
	{
		public string CardType { get; set; } = "";
		public string CardLabel { get; set; } = "";
		public string Date { get; set; } = "";
		public bool FutureEvent { get; set; }
		public bool Hidden { get; set; }
		public PdfCardContent CardData { get; set; } = new();
	}

	public class PdfCardContent
	{
		public List<PdfCardData>? Children { get; set; }

		public string? ImageBase64 { get; set; }

		public string? Name { get; set; }
		public string? LocationMet { get; set; }
		public string? WhatSaid { get; set; }
		public string? Relationship { get; set; }

		public string? Location { get; set; }

		public string? ExtraInfo { get; set; }

		public string? Plot { get; set; }

		public string? PP { get; set; }
		public string? GP { get; set; }
		public string? SP { get; set; }
		public string? CP { get; set; }

		public string? IName { get; set; }
		public string? ICategory { get; set; }
		public string? IRarity { get; set; }
		public string? IDescription { get; set; }
		public string? IWeight { get; set; }
		public string? IQuantity { get; set; }

		public string? WName { get; set; }
		public string? WCategory { get; set; }
		public string? WRarity { get; set; }
		public string? WDescription { get; set; }
		public string? WWeight { get; set; }
		public string? WAttackRole { get; set; }
		public string? WDamageType { get; set; }

		public string? SName { get; set; }
		public string? SLevel { get; set; }
		public string? SSchool { get; set; }
		public string? SCastingTime { get; set; }
		public string? SRange { get; set; }
		public string? STarget { get; set; }
		public string? SComponents { get; set; }
		public string? SDuration { get; set; }
		public string? SClasses { get; set; }
		public string? SDescription { get; set; }
		public string? SHigherLevels { get; set; }
	}
}