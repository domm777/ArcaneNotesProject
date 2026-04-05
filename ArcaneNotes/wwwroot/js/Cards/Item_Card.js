export function AddItem(cardData = {}) {
    const {
        IName= '',
        ICategory= '',
        IRarity= '',
        IDescription= '',
        IWeight= '',
        IQuantity= ''
    } = cardData || {};

    const uniqueId = Date.now();
    const body = (`
            <div class="item-content" data-content="item">
                <div class="col">
                    <div class="col-md-4"><input class="form-control i-name" placeholder="Item Name" value="${IName}"></div>
                    <div class="col-md-4"><input class="form-control i-cat" placeholder="Category" value="${ICategory}"></div>
                    <div class="col-md-4"><input class="form-control i-rare" placeholder="Rareity" value="${IRarity}"></div>
                    <div class="col-md-4"><input class="form-control i-desc" placeholder="Description" value="${IDescription}"></div>
                    <div class="col-md-4"><input class="form-control i-weight" placeholder="Weight" value="${IWeight}"></div>
                    <div class="col-md-4"><input class="form-control i-quant" placeholder="Quantity" value="${IQuantity}"></div>
                </div>
            </div>
    `);
    return {CardName: "Item", CardBody: body, HasImage: true};
}

export function ExportItem(container) {
    return {
        IName: container.querySelector('.i-name')?.value || '',
        ICategory: container.querySelector('.i-cat')?.value || '',
        IRarity: container.querySelector('.i-rare')?.value || '',
        IDescription: container.querySelector('.i-desc')?.value || '',
        IWeight: container.querySelector('.i-weight')?.value || '',
        IQuantity: container.querySelector('.i-quant')?.value || ''
    };
}