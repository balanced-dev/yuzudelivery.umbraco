StimulusApp.register('comments', class extends Stimulus.Controller {
	static get targets() {
		return [
			'list',
			'form'
		];
	};

	initialize() {
        this.settings = JSON.parse(this.data.get('settings'));
    }

    refreshComments(e) {
		e.preventDefault();

        this.toggleLoadingState();
        var formRequestInfo = {};
        var formUrl = this.settings.commentFormUrl;
        if (!StimulusApp.settings.isFrontend) {
            var formData = this.getFormData();
            formRequestInfo = { method: 'POST', body: formData };
            formUrl = this.formAction;
            if (!formUrl.endsWith(this.settings.commentFormUrl))
                formUrl = formUrl + this.settings.commentFormUrl;
        }

        this.sendRequest(this.formTarget, formUrl, formRequestInfo)
            .then(() => this.sendRequest(this.listTarget, this.settings.commentListUrl))
            .then(() => this.toggleLoadingState());
	}
	
	sendRequest(element, url, data) {
		return fetch(url, data)
			.then(response => response.text())
			.then((html) => this.handleResponse(element, html));
	}

	async handleResponse(element, html) {
		await StimulusApp.utilites.simulateNetworkDelay(() => this.updateSectionHtml(element, html));
	}

	updateSectionHtml(element, htmlContent) {
		element.innerHTML = htmlContent;
	}

	toggleLoadingState() {
		this.element.classList.toggle(this.settings.loadingClass);
	}

	getFormData() {
        let form = this.formTarget.querySelector('form');
        this.formAction = form.getAttribute('action');
        return new FormData(form);
    }

}); 