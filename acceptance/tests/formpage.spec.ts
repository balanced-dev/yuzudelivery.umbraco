import { ConstantHelper } from '@umbraco/playwright-testhelpers';
import { test } from '../helpers/YuzuTestExtension'
import { expect } from '@playwright/test';

test('formpage renders correctly', async ({ page, umbracoUi, yuzuUi }) => {
  await page.goto('/formpage');
  await expect(page).toHaveScreenshot({fullPage: true});
});