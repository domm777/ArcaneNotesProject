import { SetUpCard } from "../Cards/CardInitializer.js";

export function LoadFromJsonData(data) {
    if (!data) return;
    ClearPage();
    let payload = data.noteData || data.NoteData;
    // Intercept the default "{}" object seeded by the C# backend
    if (!payload || (typeof payload === 'object' && Object.keys(payload).length === 0 && !Array.isArray(payload))) {
        console.log("Initial empty workspace state detected. Bailing out early.");
        return;
    }

    // Handle potential double-serialization from the API
    if (typeof payload === 'string') {
        try {
            payload = JSON.parse(payload);
        } catch (e) {
            console.error("Critical: Failed to parse noteData string from DB.", e);
            return;
        }
    }

    const safeArray = Array.isArray(payload) ? payload : [];
    if (safeArray.length === 0) return;
    
    const formContainer = document.getElementById("formContainer");
    importCardData(safeArray, formContainer);
}

export function ImportJson() {
    const input = document.createElement("input");
    input.type = "file";
    input.accept = "application/json";
    input.style.display = "none";

    input.onchange = async (e) => {
        const file = e.target.files?.[0];
        if (!file) return;

        try {
            const text = await file.text();
            const data = JSON.parse(text);

            LoadFromJsonData(data);
            console.log("Import successful!");
        } catch (error) {
            console.error("JSON Parse Error:", error);
            alert("Malformed JSON file. Check the console for details.");
        } finally {
            input.remove();
        }
    };
    
    document.body.appendChild(input);
    input.click();
}

export function importCardData(cardsArray, containerElement) {
    cardsArray.forEach((noteInfo) => {
        const cardType = noteInfo.CardType;
        SetUpCard(containerElement, cardType, noteInfo);

        if (cardType === "Group_Card") {
            if (
                noteInfo.CardData?.Children &&
                noteInfo.CardData.Children.length > 0
            ) {
                // wait for dom to update before processing childern
                new Promise(resolve => setTimeout(resolve, 50));
                setTimeout(() => {
                    const allGroups = containerElement.querySelectorAll(".group-items");
                    const newGroup = allGroups[allGroups.length - 1];
                    importCardData(noteInfo.CardData.Children, newGroup);
                }, 50);
            }
        }
    });
}

export function ClearPage() {
    const formContainer = document.getElementById("formContainer");
    if (formContainer) {
        formContainer.innerHTML = "";
    }
}
