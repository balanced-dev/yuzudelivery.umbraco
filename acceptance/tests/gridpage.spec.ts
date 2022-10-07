import { test, expect } from '@playwright/test';

test('gridpage renders correctly', async ({ page }) => {
  await page.goto('/gridpage');
  await expect(page).toHaveScreenshot();
});
