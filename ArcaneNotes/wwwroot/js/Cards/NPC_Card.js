function esc(value = '') {
    return String(value)
        .replace(/&/g, '&amp;')
        .replace(/"/g, '&quot;')
        .replace(/</g, '&lt;')
        .replace(/>/g, '&gt;');
}

export function AddNPC(isGM = false, cardData = {}) {
    const {
        Name = '',
        LocationMet = '',
        WhatSaid = '',
        Relationship = ''
    } = cardData || {};

    const relationshipField = isGM ? `
        <div class="spellbook-field">
            <label class="spellbook-label">Relationship</label>
            <input type="text"
                   class="spellbook-input npc-rel"
                   placeholder="Relationship to the party"
                   value="${esc(Relationship)}">
        </div>
    ` : '';

    const body = `
        <div class="spellbook-grid">
            <div class="spellbook-field">
                <label class="spellbook-label">Name</label>
                <input type="text"
                       class="spellbook-input npc-name"
                       placeholder="NPC name"
                       value="${esc(Name)}">
            </div>

            <div class="spellbook-field">
                <label class="spellbook-label">Location Met</label>
                <input type="text"
                       class="spellbook-input npc-location"
                       placeholder="Where the party met them"
                       value="${esc(LocationMet)}">
            </div>

            ${relationshipField}

            <div class="spellbook-field spellbook-field-wide">
                <label class="spellbook-label">${isGM ? 'What They Spoke About' : 'Notes'}</label>
                <textarea class="spellbook-textarea npc-spoke"
                          placeholder="${isGM ? 'Important conversation details' : 'Notes about this person'}">${esc(WhatSaid)}</textarea>
            </div>
        </div>
    `;

    return { CardName: "NPC", CardBody: body, HasImage: true };
}

export function ExportNPC(container) {
    return {
        Name: container.querySelector('.npc-name')?.value || '',
        LocationMet: container.querySelector('.npc-location')?.value || '',
        WhatSaid: container.querySelector('.npc-spoke')?.value || '',
        Relationship: container.querySelector('.npc-rel')?.value || ''
    };
}