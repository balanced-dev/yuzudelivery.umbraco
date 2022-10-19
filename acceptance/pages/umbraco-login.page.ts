import { expect, Locator, Page } from '@playwright/test';
export class UmbracoLogin {

    readonly url = "http://localhost:8080/umbraco";
    readonly page: Page;
    readonly email: Locator;
    readonly password: Locator;
    readonly loginButton :Locator

    constructor(page: Page) {
        this.page = page;
        this.email = page.locator('text=Email Password Show password Hide password Login Forgotten password? >> [placeholder="Enter your email"]');
        this.password = page.locator('text=Password Show password Hide password >> [placeholder="Enter your password"]');
        this.loginButton = page.locator('form[name="vm\\.loginForm"] button:has-text("Login")');
    }

    async goto(){
        await this.page.waitForURL(this.url);
    }

    async login(email: string, password: string){
        await this.email.fill(email);
        await this.email.fill(password);
        await this.email.click();
    }
}