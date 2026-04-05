import {GetCurrentNoteData} from "./ExportCards.js";

export async function SaveCards(workPlaceId, userId){
    const rawData = GetCurrentNoteData();
    // Safety check: if it's already a string, don't double-stringify it
    const jsonData = JSON.stringify(rawData, null, 2);
    console.log(jsonData);
    // Added a leading slash to ensure it routes from the root domain
    const url = `/Workspace/Save/${workPlaceId}/${userId}`;

    try {
        const response = await fetch(url, {
            method: 'POST',
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json' // <-- CRITICAL: Tells C# to parse the body as JSON
            },
            body: jsonData
        });

        if (!response.ok) {
            console.error(`Network response failed. Status: ${response.status}`);
            return;
        }
        console.log("Saved");
    } catch (error) {
        console.error('Fetch error:', error);
    }
}