import { GetCurrentNoteData } from "./ExportCards.js";

function showPopup(message) {
    const popup = document.getElementById("arcanePopup");
    const msg = document.getElementById("popupMessage");
    const closeBtn = document.getElementById("popupClose");

    if (!popup || !msg || !closeBtn) return;

    msg.textContent = message;
    popup.style.display = "flex";

    closeBtn.onclick = () => {
        popup.style.display = "none";
    };

    popup.onclick = (e) => {
        if (e.target === popup) {
            popup.style.display = "none";
        }
    };
}

export async function ExportPDF(role = "GM") {
    const today = new Date();
    const dateString = today
        .toLocaleDateString("en-GB", {
            day: "2-digit",
            month: "2-digit",
            year: "numeric"
        })
        .replace(/\//g, "-");

    const currentNote = GetCurrentNoteData();

    const hasCards = Array.isArray(currentNote?.NoteData) && currentNote.NoteData.length > 0;
    const hasHeader =
        (currentNote?.Title || "").trim() !== "" ||
        (currentNote?.Name || "").trim() !== "";

    if (!hasCards && !hasHeader) {
        showPopup("⚠️ You fool! Add something.");
        return;
    }

    const payload = {
        role,
        date: dateString,
        title: currentNote.Title || "",
        name: currentNote.Name || "",
        type: currentNote.Type || "gm",
        noteData: currentNote.NoteData || []
    };

    try {
        const res = await fetch("/Notes/ExportPdf", {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(payload)
        });

        if (!res.ok) {
            const errorText = await res.text();
            console.error("PDF generation failed:", errorText);
            showPopup("PDF generation failed. Please try again.");
            return;
        }

        const blob = await res.blob();
        const url = URL.createObjectURL(blob);

        const a = document.createElement("a");
        a.href = url;
        a.download = `${dateString}.pdf`;
        document.body.appendChild(a);
        a.click();
        a.remove();

        URL.revokeObjectURL(url);
    } catch (error) {
        console.error("PDF export crashed:", error);
        showPopup("PDF export crashed. Check the console for details.");
    }
}