using ArcaneNotes.Models;
using DnDSessionNotes.Models;
using Microsoft.AspNetCore.Mvc;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Globalization;

namespace DnDSessionNotes.Controllers
{
	[IgnoreAntiforgeryToken]
	public class NotesController : Controller
	{
		[HttpGet("/Home/GMForm")]
		public IActionResult GMForm() => View("~/Views/Home/GMForm.cshtml");

		[HttpGet("/Home/PlayerForm")]
		public IActionResult PlayerForm() => View("~/Views/Home/PlayerForm.cshtml");

		[HttpGet("/Home/WelcomeScreen")]
		public IActionResult WelcomeScreen() => View("~/Views/Home/WelcomeScreen.cshtml");

		[HttpGet("/Login/Register")]
		public IActionResult Register() => View("~/Views/Login/Register.cshtml");

		[HttpGet("/Login/Login")]
		public IActionResult Login() => View("~/Views/Login/Login.cshtml");

		[HttpPost]
		[Route("/Notes/ExportPdf")]
		public IActionResult ExportPdf([FromBody] PdfExportPayload payload)
		{
			payload ??= new PdfExportPayload();
			QuestPDF.Settings.License = LicenseType.Community;

			var today = DateTime.Now;
			var formattedDate = today.ToString("dd-MM-yyyy", CultureInfo.InvariantCulture);

			var pdf = Document.Create(container =>
			{
				container.Page(page =>
				{
					page.Size(PageSizes.A4);
					page.Margin(32);
					page.PageColor(Colors.White);
					page.DefaultTextStyle(x => x.FontSize(11).FontColor(Colors.Grey.Darken4));

					page.Header().Column(col =>
					{
						col.Item().Text(string.IsNullOrWhiteSpace(payload.Title) ? "Session Notes" : payload.Title)
							.FontSize(20)
							.Bold()
							.AlignCenter();

						if (!string.IsNullOrWhiteSpace(payload.Name))
							col.Item().Text($"Name: {payload.Name}");

						col.Item().PaddingTop(5)
							.Text($"Note Made: {DateTime.Now:F}")
							.FontSize(10)
							.Italic();

						col.Item().PaddingTop(10).LineHorizontal(1f);
					});

					page.Content().PaddingTop(10).Column(col =>
					{
						if (payload.NoteData == null || !payload.NoteData.Any())
						{
							col.Item().PaddingTop(20).Text("No note content found.")
								.FontSize(12)
								.Italic()
								.FontColor(Colors.Grey.Darken1);
							return;
						}

						RenderCards(col, payload.NoteData, 0);
					});

					page.Footer().AlignCenter().Text(text =>
					{
						text.Span("Page ").FontSize(10).FontColor(Colors.Grey.Darken1);
						text.CurrentPageNumber().FontSize(10).SemiBold();
						text.Span(" of ").FontSize(10).FontColor(Colors.Grey.Darken1);
						text.TotalPages().FontSize(10).SemiBold();
					});
				});
			});

			var stream = new MemoryStream();
			pdf.GeneratePdf(stream);
			stream.Position = 0;

			var safeTitle = string.IsNullOrWhiteSpace(payload.Title)
				? "SessionNotes"
				: string.Concat(payload.Title.Split(Path.GetInvalidFileNameChars()));

			return File(stream, "application/pdf", $"{safeTitle}-{formattedDate}.pdf");

		}

		private static void RenderCards(ColumnDescriptor col, List<PdfCardData> cards, int depth)
		{
			foreach (var card in cards)
			{
				var cleanType = (card.CardType ?? "Card").Replace("_Card", "").Replace("_", " ");
				var title = !string.IsNullOrWhiteSpace(card.CardLabel)
					? $"{cleanType}: {card.CardLabel}"
					: cleanType;

				col.Item().PaddingBottom(8).Border(1).Padding(8).Column(c =>
				{
					c.Item().Text(title)
						.Bold()
						.FontSize(Math.Max(12 - depth, 10));

					if (!string.IsNullOrWhiteSpace(card.Date))
						c.Item().Text($"Date: {card.Date}");

					if (card.FutureEvent)
						c.Item().Text("Future Event: Yes");

					RenderCardContent(c, card);

					if (card.CardType == "Group_Card" && card.CardData?.Children?.Any() == true)
					{
						c.Item().PaddingTop(6).Text("Grouped Items").Bold();
						RenderCards(c, card.CardData.Children, depth + 1);
					}
				});
			}
		}

		private static void RenderCardContent(ColumnDescriptor c, PdfCardData card)
		{
			var data = card.CardData;
			if (data == null) return;

			switch (card.CardType)
			{
				case "NPC_Card":
					AddField(c, "Name", data.Name);
					AddField(c, "Location Met", data.LocationMet);
					AddParagraph(c, "Notes", data.WhatSaid);
					AddField(c, "Relationship", data.Relationship);
					AddImageIfExists(c, data.ImageBase64);
					break;

				case "Location_Card":
					AddParagraph(c, "Location", data.Location);
					AddImageIfExists(c, data.ImageBase64);
					break;

				case "ExtraInfo_Card":
					AddParagraph(c, "Extra Info", data.ExtraInfo);
					AddImageIfExists(c, data.ImageBase64);
					break;

				case "Plot_Card":
					AddParagraph(c, "Plot", data.Plot);
					AddImageIfExists(c, data.ImageBase64);
					break;

				case "Treasure_Card":
					c.Item().Text($"PP: {data.PP ?? "0"}    GP: {data.GP ?? "0"}    SP: {data.SP ?? "0"}    CP: {data.CP ?? "0"}")
						.FontSize(11);
					AddImageIfExists(c, data.ImageBase64);
					break;

				case "Item_Card":
					RenderItemCard(c, data);
					AddImageIfExists(c, data.ImageBase64);
					break;
			}
		}

		private static void RenderItemCard(ColumnDescriptor c, PdfCardContent data)
		{
			switch (data.SelectedType)
			{
				case "weapon":
					AddField(c, "Weapon", data.WeaponData?.WName);
					AddField(c, "Category", data.WeaponData?.WCategory);
					AddField(c, "Rarity", data.WeaponData?.WRarity);
					AddParagraph(c, "Description", data.WeaponData?.WDescription);
					AddField(c, "Weight", data.WeaponData?.WWeight);
					AddField(c, "Attack Roll", data.WeaponData?.WAttackRole);
					AddField(c, "Damage Type", data.WeaponData?.WDamageType);
					break;

				case "spell":
					AddField(c, "Spell", data.SpellData?.SName);
					AddField(c, "Level", data.SpellData?.SLevel);
					AddField(c, "School", data.SpellData?.SSchool);
					AddField(c, "Casting Time", data.SpellData?.SCastingTime);
					AddField(c, "Range", data.SpellData?.SRange);
					AddField(c, "Target", data.SpellData?.STarget);
					AddField(c, "Components", data.SpellData?.SComponents);
					AddField(c, "Duration", data.SpellData?.SDuration);
					AddField(c, "Classes", data.SpellData?.SClasses);
					AddParagraph(c, "Description", data.SpellData?.SDescription);
					AddParagraph(c, "Higher Levels", data.SpellData?.SHigherLevels);
					break;

				default:
					AddField(c, "Item", data.ItemData?.IName);
					AddField(c, "Category", data.ItemData?.ICategory);
					AddField(c, "Rarity", data.ItemData?.IRarity);
					AddParagraph(c, "Description", data.ItemData?.IDescription);
					AddField(c, "Weight", data.ItemData?.IWeight);
					AddField(c, "Quantity", data.ItemData?.IQuantity);
					break;
			}
		}

		private static void AddField(ColumnDescriptor c, string label, string? value)
		{
			if (string.IsNullOrWhiteSpace(value))
				return;

			c.Item().PaddingTop(2).Text(text =>
			{
				text.Span($"{label}: ").SemiBold().FontColor(Colors.Black);
				text.Span(value).FontColor(Colors.Black);
			});
		}

		private static void AddParagraph(ColumnDescriptor c, string label, string? value)
		{
			if (string.IsNullOrWhiteSpace(value))
				return;

			c.Item().PaddingTop(2).Text(text =>
			{
				text.Span($"{label}: ").SemiBold().FontColor(Colors.Black);
				text.Span(value).FontColor(Colors.Black);
			});
		}

		private static void AddImageIfExists(ColumnDescriptor column, string? base64Image)
		{
			if (string.IsNullOrWhiteSpace(base64Image))
				return;

			try
			{
				var imgData = base64Image.Contains(',')
					? base64Image.Split(',')[1]
					: base64Image;

				var imgBytes = Convert.FromBase64String(imgData);

				column.Item()
					.PaddingTop(5)
					.AlignCenter()
					.Height(120)
					.Image(imgBytes)
					.FitArea();
			}
			catch
			{
				column.Item().Text("[Image failed to load]").FontColor(Colors.Red.Medium);
			}
		}


	}
}
