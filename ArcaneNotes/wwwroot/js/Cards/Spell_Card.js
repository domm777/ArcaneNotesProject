export function AddSpell(cardData = {}) {
    const {
        SName = '',
        SLevel = '',
        SSchool = '',
        SCastingTime = '',
        SRange = '',
        STarget = '',
        SComponents = '',
        SDuration = '',
        SClasses = '',
        SDescription = '',
        SHigherLevels = ''
    } = cardData || {};
    
    const body = (`
            <div data-content="spell">
                <div class="col">
                    <div class="col-md-4"><input class="form-control s-name" placeholder="Spell Name" value="${SName}"></div>
                    <div class="col-md-4"><input class="form-control s-level" placeholder="Level" value="${SLevel}"></div>
                    <div class="col-md-4"><input class="form-control s-school" placeholder="School" value="${SSchool}"></div>
                    <div class="col-md-4"><input class="form-control s-time" placeholder="Casting Time" value="${SCastingTime}"></div>
                    <div class="col-md-4"><input class="form-control s-range" placeholder="Range" value="${SRange}"></div>
                    <div class="col-md-4"><input class="form-control s-target" placeholder="Target" value="${STarget}"></div>
                    <div class="col-md-4"><input class="form-control s-comp" placeholder="Components" value="${SComponents}"></div>
                    <div class="col-md-4"><input class="form-control s-duration" placeholder="Duration" value="${SDuration}"></div>
                    <div class="col-md-4"><input class="form-control s-classes" placeholder="Classes" value="${SClasses}"></div>
                    <div class="col-md-4"><input class="form-control s-desc" placeholder="Description" value="${SDescription}"></div>
                    <div class="col-md-4"><input class="form-control s-high" placeholder="At Higher Levels" value="${SHigherLevels}"></div>
                </div>
            </div>
    `);
    return {CardName: "Spell", CardBody: body, HasImage: true};
}

export function ExportSpell(container) {
    return {
        SName: container.querySelector('.s-name')?.value || '',
        SLevel: container.querySelector('.s-level')?.value || '',
        SSchool: container.querySelector('.s-school')?.value || '',
        SCastingTime: container.querySelector('.s-time')?.value || '',
        SRange: container.querySelector('.s-range')?.value || '',
        STarget: container.querySelector('.s-target')?.value || '',
        SComponents: container.querySelector('.s-comp')?.value || '',
        SDuration: container.querySelector('.s-duration')?.value || '',
        SClasses: container.querySelector('.s-classes')?.value || '',
        SDescription: container.querySelector('.s-desc')?.value || '',
        SHigherLevels: container.querySelector('.s-high')?.value || ''
    };
}