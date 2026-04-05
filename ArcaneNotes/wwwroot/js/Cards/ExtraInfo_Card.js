export function AddExtra(cardData = {}) {
    const{
        ExtraInfo = ''
    } = cardData || {};
    const body = (`
            <textarea class="form-control extra-text" rows="3" placeholder="Quick note">${ExtraInfo}</textarea>
        `);
    return {CardName: "Extra", CardBody: body, HasImage: true};
}

export function ExportExtraInfo(container){
    return {
        ExtraInfo: container.querySelector('.extra-text')?.value || ''
    };
}