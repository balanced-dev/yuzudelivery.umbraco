import { Locator, Page } from '@playwright/test';
import { Url } from 'url';
export class UmbracoMarketing {

    readonly page: Page;
    readonly url: string;
    readonly closeTourButton :Locator;
    readonly closeMarketingBannerButton :Locator;

    constructor(page: Page) {
        this.page = page;
        this.url = `${process.env.URL}/umbraco`;
        this.closeTourButton = this.page.locator('text=Don\'t show this tour again');
        this.closeMarketingBannerButton = this.page.locator('role=button[name="No thanks"]');

    }

    async clear(){

        await this.page.goto(this.url);

        if(await this.closeTourButton.count() > 0) {
            await this.closeTourButton.click();
        }

        if(await this.closeMarketingBannerButton.count() > 0) {
            await this.closeMarketingBannerButton.click();
        }


    }
}