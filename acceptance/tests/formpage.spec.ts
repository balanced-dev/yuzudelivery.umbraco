import { test, expect } from '@playwright/test';

test('formpage renders correctly', async ({ page }) => {

  // Go to http://localhost:8080/umbraco
  await page.goto('http://localhost:8080/umbraco');
  // Fill text=Email Password Show password Hide password Login Forgotten password? >> [placeholder="Enter your email"]
  await page.locator('text=Email Password Show password Hide password Login Forgotten password? >> [placeholder="Enter your email"]').fill('e2e@hifi.agency');

  // Fill text=Password Show password Hide password >> [placeholder="Enter your password"]
  await page.locator('text=Password Show password Hide password >> [placeholder="Enter your password"]').fill('TestThis42!');

  // Click form[name="vm\.loginForm"] button:has-text("Login")
  await page.locator('form[name="vm\\.loginForm"] button:has-text("Login")').click();

  const tour = page.locator('text=Don\'t show this tour again');
  if(tour) {
    tour.click();
  }

  // Go to http://localhost:8080/umbraco#/forms/Form/edit/-1?create=true&template=contactform
  await page.goto('http://localhost:8080/umbraco#/forms/Form/edit/-1?create=true&template=contactform');

  // Click button:has-text("Save")
  await page.locator('button:has-text("Save")').click();

  // Click text=Content
  await page.locator('text=Content').click();

  // Click button:has-text("Expand child items for Home")
  await page.locator('button:has-text("Expand child items for Home")').click();

  await page.locator('a:has-text("FormPage")').click();

  await page.locator('button:has-text("Add")').click();

  // Click a:has-text("Contact form")
  await page.locator('a:has-text("Contact form")').click();

  // Click button:has-text("Save and publish")
  await page.locator('button:has-text("Save and publish")').click();

  await page.locator('text=Content published: and is visible on the website');

  await page.locator('text=Info !').click();

  // Click text=Links /formpage/ >> a
  const [page1] = await Promise.all([
    page.waitForEvent('popup'),
    page.locator('text=Links /formpage/ >> a').click(),
    page.locator('text=Contact Us')
  ]);

  //await page1.screenshot();

  await expect(page1).toHaveScreenshot();


});
