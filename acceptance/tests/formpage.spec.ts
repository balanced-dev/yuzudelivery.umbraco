import {test} from '@umbraco/playwright-testhelpers';
import {
  ContentBuilder,
  DocumentTypeBuilder,
  PartialViewMacroBuilder,
  MacroBuilder,
  GridDataTypeBuilder
} from "@umbraco/json-models-builders";

import { expect } from '@playwright/test';

test.describe('Content tests', () => {

    test.beforeEach(async ({page, umbracoApi}) => {
      // TODO: REMOVE THIS WHEN SQLITE IS FIXED
      // Wait so we don't bombard the API
      await page.waitForTimeout(1000);
      await umbracoApi.login(true);
    });


    test('formpage renders correctly', async ({ page }) => {

      // Go to http://localhost:8080/umbraco#/forms/Form/edit/-1?create=true&template=contactform
      await page.goto('http://localhost:8080/umbraco#/forms/Form/edit/-1?create=true&template=contactform');

      // Click button:has-text("Save")
      /*await page.locator('button:has-text("Save")').click();

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

      await expect(page1).toHaveScreenshot();*/


    });

  });
