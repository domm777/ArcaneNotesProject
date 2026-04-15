import { AddNPC } from './NPC_Card.js';
import { AddLocation } from "./Location_Card.js";
import { AddExtra } from "./ExtraInfo_Card.js";
import { AddGroup } from "./Group_Card.js";
import { AddItem } from "./Item_Card.js";
import { AddPlot } from "./Plot_Card.js";
import { AddTreasure } from "./Treasure_Card.js";
import {AddSpell} from "./Spell_Card.js";
import {AddWeapon} from "./Weapon_Card.js";

// -----------------------------
// Helper to build DOM nodes
// -----------------------------
function el(html) {
    const t = document.createElement('template');
    t.innerHTML = html.trim();
    return t.content.firstElementChild;
}

// -----------------------------
// IMAGE UPLOAD & DROP
// -----------------------------
function enableImageDrop(preview) {
    preview.addEventListener('dragover', (e) => {
        e.preventDefault();
        preview.classList.add('drag-hover');
    });

    preview.addEventListener('dragleave', () => {
        preview.classList.remove('drag-hover');
    });

    preview.addEventListener('drop', (e) => {
        e.preventDefault();
        preview.classList.remove('drag-hover');
        const file = e.dataTransfer.files[0];
        handleFile(file, preview);
    });
}

function triggerImageUpload(preview) {
    const input = document.createElement('input');
    input.type = 'file';
    input.accept = 'image/*';
    input.style.display = 'none';
    input.onchange = (e) => handleFile(e.target.files[0], preview);
    input.click();
}

function handleFile(file, preview) {
    if (!file) return;

    const reader = new FileReader();
    reader.onload = (e) => {
        preview.innerHTML = '';

        const img = document.createElement('img');
        img.src = e.target.result;
        img.className = 'preview-img';
        img.dataset.imageBase64 = e.target.result;

        preview.appendChild(img);
    };

    reader.readAsDataURL(file);
}

function imageCard(spot) {
    const targetSpot = spot.querySelector('.image-spot');
    if (!targetSpot || targetSpot.querySelector('.image-upload-wrapper')) return;

    const wrapper = el(`
        <div class="image-upload-wrapper position-relative">
            <div class="imageUploadDiv" style="cursor:pointer;">
                <p class="image-placeholder mb-0">
                    Click or drop an image here
                </p>
            </div>
            <button
                type="button"
                class="CameraButtonCancel"
                style="z-index:2;"
                title="Remove image"
                onclick="event.stopPropagation(); this.closest('.image-upload-wrapper').remove()">
                ✖
            </button>
        </div>
    `);

    const uploadDiv = wrapper.querySelector('.imageUploadDiv');
    enableImageDrop(uploadDiv);

    uploadDiv.onclick = function () {
        triggerImageUpload(uploadDiv);
    };

    targetSpot.appendChild(wrapper);
}

export function SetUpCard(appendLocation, cardType, noteData = {}) {
    const {
        CardLabel = '',
        Date = '',
        FutureEvent = false,
        Hidden = false,
        CardData = {}
    } = noteData || {};

    const cards = {
        NPC_Card: AddNPC(true, CardData),
        Location_Card: AddLocation(CardData),
        ExtraInfo_Card: AddExtra(CardData),
        Item_Card: AddItem(CardData),
        Spell_Card: AddSpell(CardData),
        Weapon_Card: AddWeapon(CardData),
        Plot_Card: AddPlot(CardData),
        Group_Card: AddGroup(CardData),
        Treasure_Card: AddTreasure(CardData),
    };

    const createdCard = cards[cardType];
    if (!createdCard) return;

    const block = el(`
        <div class="card-container spellbook-card" id="${cardType}">
            <div class="spellbook-card-header">
                <div class="spellbook-card-header-left">
                    <span class="drag-handle" title="Drag card">☰</span>
                    <h5 class="spellbook-card-title">${createdCard.CardName}</h5>
                </div>

                <div class="spellbook-card-header-right">
                    ${createdCard.HasImage ? `<button type="button" class="card-action-btn img-btn" title="Add image">📷</button>` : ''}
                    <button type="button" class="card-action-btn collapse-toggle" title="Collapse">−</button>
                    <button type="button" class="delete-btn" title="Delete" onclick="this.closest('.card-container').remove()">✖</button>
                </div>
            </div>

            <div class="spellbook-field">
                
                <input type="text" class="spellbook-input card-label" placeholder="Custom Label" value="${CardLabel}">
            </div>

            <div class="card-body-collapsible">
                <hr>
                ${createdCard.CardBody}

               <div class="bottom-meta-row">
    <input type="date" class="spellbook-input spellbook-date-input" value="${Date}">
    
    <div class="spellbook-checkbox-wrap">
    <label class="spellbook-toggle">
        <input type="checkbox" class="future-event-checkbox" ${FutureEvent ? "checked" : ""}>
        <span class="spellbook-slider"></span>
    </label>
    <span class="spellbook-toggle-label">Future Event</span>
</div>
<div class="spellbook-checkbox-wrap">
    <label class="spellbook-toggle">
        <input type="checkbox" class="hidden-event-checkbox" ${Hidden ? "checked" : ""}>
        <span class="spellbook-slider"></span>
    </label>
    <span class="spellbook-toggle-label">Hide Card</span>
</div>
</div>

                ${createdCard.HasImage ? `<div class="image-spot"></div>` : ''}
            </div>
        </div>
    `);

    appendLocation.appendChild(block);

    const labelInput = block.querySelector('.card-label');
    const collapseToggle = block.querySelector('.collapse-toggle');
    const collapsibleBody = block.querySelector('.card-body-collapsible');
    const imgBtn = block.querySelector('.img-btn');

    function updateLabelState() {
        const isCollapsed = collapsibleBody.style.display === 'none';
        const hasValue = labelInput.value.trim() !== '';

        if (isCollapsed) {
            labelInput.setAttribute('readonly', true);
            labelInput.placeholder = hasValue ? 'Custom Label' : '';
        } else {
            labelInput.removeAttribute('readonly');
            labelInput.placeholder = 'Custom Label';
        }
    }

    collapseToggle.addEventListener('click', () => {
        const isHidden = collapsibleBody.style.display === 'none';
        collapsibleBody.style.display = isHidden ? '' : 'none';
        collapseToggle.textContent = isHidden ? '−' : '+';
        setTimeout(updateLabelState, 0);
    });

    if (imgBtn) {
        imgBtn.addEventListener('click', () => {
            imageCard(block);
        });
    }

    labelInput.addEventListener('input', updateLabelState);

    const innerList = block.querySelector('.group-items');
    if (innerList != null) {
        Sortable.create(innerList, {
            group: 'shared-lists',
            animation: 150,
            handle: '.drag-handle',
            emptyInsertThreshold: 50,
            ghostClass: 'bg-light',
            onAdd: function (evt) {
                const msg = evt.to.querySelector('.empty-msg');
                if (msg) msg.style.display = 'none';
            }
        });
    }
}