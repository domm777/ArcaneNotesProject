export function AddLocation(cardData = {}) {
    const {
        Location = ''
    } = cardData || {};
    const body = (`
            <input class="form-control location-text" placeholder="Where you went or met someone", value="${Location}">
        `);
    return {CardName: "Location", CardBody: body, HasImage: true}
}

export function ExportLocation(container){
    return {
        Location: container.querySelector('.location-text')?.value || ''
    };
}