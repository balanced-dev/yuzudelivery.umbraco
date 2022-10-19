import { expect, Locator, Page } from '@playwright/test';
export class UmbracoLogin {

    readonly url = "http://localhost:8080/umbraco";
    readonly page: Page;

    readonly email: Locator;
    readonly password: Locator;
    readonly loginButton :Locator

    readonly closeTourButton :Locator;
    readonly closeMarketingBannerButton :Locator;

    constructor(page: Page) {
        this.page = page;

        this.email = this.page.locator('text=Email Password Show password Hide password Login Forgotten password? >> [placeholder="Enter your email"]');
        this.password = this.page.locator('text=Password Show password Hide password >> [placeholder="Enter your password"]');
        this.loginButton = this.page.locator('form[name="vm\\.loginForm"] button:has-text("Login")');
        this.closeTourButton = this.page.locator('text=Don\'t show this tour again');
        this.closeMarketingBannerButton = this.page.locator('role=button[name="No thanks"]');

    }

    async goto(){
        await this.page.goto(this.url);
    }

    async login(email: string, password: string){

        await this.email.fill(email);
        await this.password.fill(password);
        await this.loginButton.click();

        if(await this.closeTourButton.count() > 0) {
            await this.closeTourButton.click();
        }

        if(wait this.closeMarketingBannerButton.count() > 0) {
            await this.closeMarketingBannerButton.click();
        }


    }
}