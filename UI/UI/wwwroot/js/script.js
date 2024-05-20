(function () {
    // use a custom namespace to avoid polluting the window object
    window.myNamespace = {
        bootstrap: {
            modals: {
                show: el => {
                    // I will be passing in the element itself, 
                    // but the selector could also be used
                    const instance = bootstrap.Modal.getOrCreateInstance(el);
                    instance.show();
                }
            }
        }
    };
})();