export function AddTreasure(cardData = {}) {
    const {
        PP = '',
        GP = '',
        SP = '',
        CP = ''
    } = cardData || {};
    
    const body = (`
            <div class="row g-2 align-items-center mb-2">
                <div class="col-md-2 d-flex align-items-center">
                    <input type="number" class="form-control treasure-plat pp" placeholder="0" min="0" value="${PP}">
                    <span class="ms-2 text-muted">pp</span>
                </div>
                <div class="col-md-2 d-flex align-items-center">
                    <input type="number" class="form-control treasure-gold gp" placeholder="0" min="0" value="${GP}">
                    <span class="ms-2 text-muted">gp</span>
                </div>
                <div class="col-md-2 d-flex align-items-center">
                    <input type="number" class="form-control treasure-silver sp" placeholder="0" min="0" value="${SP}">
                    <span class="ms-2 text-muted">sp</span>
                </div>
                <div class="col-md-2 d-flex align-items-center">
                    <input type="number" class="form-control treasure-copper cp" placeholder="0" min="0" value="${CP}">
                    <span class="ms-2 text-muted">cp</span>
                </div>
            </div>
        `);
    return {CardName: "Treasure", CardBody: body, HasImage: true}
}

export function ExportTreasure(container){
    return {
        PP: container.querySelector('.pp')?.value || '',
        GP: container.querySelector('.gp')?.value || '',
        SP: container.querySelector('.sp')?.value || '',
        CP: container.querySelector('.cp')?.value || ''
    };
}