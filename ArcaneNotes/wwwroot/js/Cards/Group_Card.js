export function AddGroup(cardData = {}) {
    const uniqueId = 'group-' + Math.random().toString(36).substring(2, 9);
    const body = (`
        <div id="${uniqueId}" class="group-items p-2 border-dark rounded bg-opacity-25 bg-black align-content-center" style="min-height: 100px;">
                <p class="text-muted small my-2 empty-msg text-white pointer-events-none">Drop items here</p>
            </div>
    `);
    return {CardName: "Category", CardBody: body, HasImage: false}
}