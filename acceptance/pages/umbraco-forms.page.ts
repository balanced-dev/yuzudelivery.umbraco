import { Locator, Page } from '@playwright/test';
export class UmbracoForms {

    readonly page: Page;
    readonly createUrl = '/umbraco#/forms/Form/edit/-1?create=true&template=contactform';

    readonly submitButton :Locator;

    constructor(page: Page) {
        this.page = page;
        this.submitButton = this.page.locator('button:has-text("Save")');
    }

    async createContactForm(){
        await this.page.goto(this.createUrl);
        await this.submitButton.click();
    }

}