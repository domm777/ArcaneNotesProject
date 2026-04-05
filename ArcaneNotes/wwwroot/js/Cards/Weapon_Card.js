export function AddWeapon(cardData = {}) {
    const {
        WName = '',
        WCategory = '',
        WRarity = '',
        WDescription = '',
        WWeight = '',
        WAttackRole = '',
        WDamageType = ''
    } = cardData || {};
    
    const body = (`
            <div class="item-content" data-content="weapon">
                 <div class="col">
                    <div class="col-md-4"><input class="form-control w-name" placeholder="Weapon Name" value="${WName}"></div>
                    <div class="col-md-4"><input class="form-control w-cat" placeholder="Category" value="${WCategory}"></div>
                    <div class="col-md-4"><input class="form-control w-rare" placeholder="Rareity" value="${WRarity}"></div>
                    <div class="col-md-4"><input class="form-control w-desc" placeholder="Description" value="${WDescription}"></div>
                    <div class="col-md-4"><input class="form-control w-weight" placeholder="Weight" value="${WWeight}"></div>
                    <div class="col-md-4"><input class="form-control w-attack" placeholder="Attack Role" value="${WAttackRole}"></div>
                    <div class="col-md-4"><input class="form-control w-dtype" placeholder="Damage Type" value="${WDamageType}"></div>
                </div>
            </div>
    `);
    return {CardName: "Weapon", CardBody: body, HasImage: true};
}

export function ExportWeapon(container) {
    return {
        WName: container.querySelector('.w-name')?.value || '',
        WCategory: container.querySelector('.w-cat')?.value || '',
        WRarity: container.querySelector('.w-rare')?.value || '',
        WDescription: container.querySelector('.w-desc')?.value || '',
        WWeight: container.querySelector('.w-weight')?.value || '',
        WAttackRole: container.querySelector('.w-attack')?.value || '',
        WDamageType: container.querySelector('.w-dtype')?.value || ''
    };
}