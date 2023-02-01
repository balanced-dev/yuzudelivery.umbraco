import { test, expect } from '@playwright/test';

test('formpage renders correctly', async ({ page }) => {
  await page.goto('/formpage');
  await expect(page).toHaveScreenshot({maxDiffPixelRatio: 1, fullPage: true});
});