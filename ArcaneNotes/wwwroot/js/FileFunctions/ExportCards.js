import { ExportNPC } from "../Cards/NPC_Card.js";
import { ExportLocation } from "../Cards/Location_Card.js";
import { ExportExtraInfo } from "../Cards/ExtraInfo_Card.js";
import { ExportItem } from "../Cards/Item_Card.js";
import { ExportPlot } from "../Cards/Plot_Card.js";
import { ExportTreasure } from "../Cards/Treasure_Card.js";
import {ExportSpell} from "../Cards/Spell_Card.js";
import {ExportWeapon} from "../Cards/Weapon_Card.js";

const cardFunctions = {
    NPC_Card: ExportNPC,
    Location_Card: ExportLocation,
    ExtraInfo_Card: ExportExtraInfo,
    Item_Card: ExportItem,
    Spell_Card: ExportSpell,
    Weapon_Card: ExportWeapon,
    Plot_Card: ExportPlot,
    Treasure_Card: ExportTreasure,
};

export function GetCurrentNoteData() {
    const allCards = getCardData(document.getElementById("formContainer"));

    return {
        NoteData: allCards
    };
}

export function ExportJson() {
    const exportData = GetCurrentNoteData();
    const json = JSON.stringify(exportData, null, 2);

    const blob = new Blob([json], { type: "application/json" });
    const url = URL.createObjectURL(blob);
    const a = document.createElement("a");
    a.href = url;
    a.download = "notes_export.json";
    a.click();

    URL.revokeObjectURL(url);
}

function getCardData(containerElement) {
    const cards = [];
    const cardContainers = containerElement.querySelectorAll(
        ":scope > .card-container",
    );

    cardContainers.forEach((container) => {
        const cardType = container.id;
        const cardObject = {
            CardType: cardType,
            CardLabel: container.querySelector(".card-label")?.value || "",
            Date: container.querySelector('input[type="date"]')?.value || "",
            // Target specific classes instead of the generic type
            FutureEvent: container.querySelector(".future-event-checkbox")?.checked || false,
            Hidden: container.querySelector(".hidden-event-checkbox")?.checked || false,
            CardData: {},
        };

        if (cardType === "Group_Card") {
            const groupItems = container.querySelector(".group-items");
            cardObject.CardData.Children = getCardData(groupItems);
        } else {
            cardObject.CardData = cardFunctions[cardType](container);

            const imageElement = container.querySelector(".preview-img");
            if (imageElement?.dataset?.imageBase64) {
                cardObject.CardData.ImageBase64 = imageElement.dataset.imageBase64;
            }
        }

        cards.push(cardObject);
    });

    return cards;
}
