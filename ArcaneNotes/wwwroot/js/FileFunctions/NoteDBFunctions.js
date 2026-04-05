import { LoadFromJsonData } from "../FileFunctions/ImportCards.js";

export async function LoadDBNote(workPlaceId, userId) {
    const url = `/Workspace/GetNoteData/${workPlaceId}/${userId}`;

    try {
        const response = await fetch(url, {
            method: 'GET',
            headers: { 'Accept': 'application/json' }
        });

        if (!response.ok) {
            console.error('Network response failed or session expired.');
            return;
        }
        const data = await response.json();
        console.log(data);
        LoadFromJsonData(data);
    } catch (error) {
        console.error('Fetch error:', error);
    }
}

//This will be used to save your notes
export function SaveDBNote(){
    
}

export async function AddUser(workplace, email){
    const url = `/Workspace/AddCollaborator/${workplace}/${email}`;

    try {
        const response = await fetch(url, {
            method: 'POST',
            headers: { 'Accept': 'application/json' }
        });

        if (!response.ok) {
            console.error('Network response failed or session expired.');
            return;
        }
        console.log("Added");
    } catch (error) {
        console.error('Fetch error:', error);
    }
}