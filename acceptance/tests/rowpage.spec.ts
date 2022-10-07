import { test, expect } from '@playwright/test';

test('rowpage renders correctly', async ({ page }) => {
  await page.goto('/rowpage');
  await expect(page).toHaveScreenshot();
});
