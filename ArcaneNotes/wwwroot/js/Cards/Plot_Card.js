export function AddPlot(cardData = {}) {
    const {
        Plot = '',
    } = cardData || {};
    const body = (`
            <textarea class="form-control plot-text" rows="3" placeholder="What important things happened?">${Plot}</textarea>
        `);
    return {CardName: "Plot", CardBody: body, HasImage: true};
}

export function ExportPlot(container){
    return {
        Plot: container.querySelector('.plot-text')?.value || ''
    };
}