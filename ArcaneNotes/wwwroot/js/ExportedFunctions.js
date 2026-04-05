import { SetUpCard } from "./Cards/CardInitializer.js";
import { ExportJson, GetCurrentNoteData } from "./FileFunctions/ExportCards.js";
import {
  ImportJson,
  ClearPage
} from "./FileFunctions/ImportCards.js";
import { ExportPDF } from "./FileFunctions/ExportPdf.js";
import {LoadDBNote, AddUser} from "./FileFunctions/NoteDBFunctions.js"
import {SaveCards} from "./FileFunctions/SaveCards.js";

function AddCard(location, cardTag, notedata = {}) {
  SetUpCard(location, cardTag, notedata);
}

function LoadNoteFromDB(workPlaceId, userId){
  LoadDBNote(workPlaceId, userId);
}

function AddNewUser(workplaceId, email){
  AddUser(workplaceId, email);
}

function ExportCards() {
  ExportJson();
}

function ImportCards() {
  ImportJson();
}

function ClearThePage() {
  ClearPage();
}
function SaveData(workspaceId, userId){
  SaveCards(workspaceId, userId);
}

async function ExportPDF_Handler(role) {
  role = role || "GM";
  ExportPDF(role);
}

const ExportedFunctions = {
  AddCard,
  ExportCards,
  ImportCards,
  ClearThePage,
  ExportPDF: ExportPDF_Handler,
  GetCurrentNoteData,
  LoadNoteFromDB,
  AddNewUser,
  SaveData
};

window.ExportedFunctions = ExportedFunctions;
export default ExportedFunctions;
