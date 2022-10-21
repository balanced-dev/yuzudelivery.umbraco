import { ConstantHelper } from '@umbraco/playwright-testhelpers';
import { test } from '../helpers/YuzuTestExtension'
import { expect } from '@playwright/test';

import { UmbracoForms } from '../pages/umbraco-forms.page';

test.describe('Content tests', () => {

  test.beforeEach(async ({ page, umbracoApi, umbracoUi, yuzuUi }) => {
    await umbracoApi.login(true);

    await page.goto('/umbraco')
    await umbracoUi.waitForTreeLoad(ConstantHelper.sections.content);
    await umbracoUi.clickElement(yuzuUi.getTreeItem('content', ["Home", "FormPage"], { force: true }), { force: true });

    await page.waitForSelector('button:has-text("Remove")', { timeout: 5000 })
      .then(async () => {
        await page.locator('button:has-text("Remove")').click();
        await umbracoUi.clickElement(umbracoUi.getButtonByLabelKey(ConstantHelper.buttons.saveAndPublish));
        await umbracoUi.isSuccessNotificationVisible();
      })
      .catch(() => { });
  });

  test('formpage renders correctly', async ({ page, umbracoUi, yuzuUi }) => {
    const umbrcoForms = new UmbracoForms(page);
    await umbrcoForms.createContactForm();

    await umbracoUi.goToSection(ConstantHelper.sections.content);
    await umbracoUi.waitForTreeLoad(ConstantHelper.sections.content);
    await umbracoUi.clickElement(yuzuUi.getTreeItem('content', ["Home", "FormPage"]));

    await page.locator('button:has-text("Add")').click();
    await page.locator('a:has-text("Contact form")').click();

    await umbracoUi.clickElement(umbracoUi.getButtonByLabelKey(ConstantHelper.buttons.saveAndPublish));
    await umbracoUi.isSuccessNotificationVisible();

    await page.locator('text=Info !').click();

    const [page1] = await Promise.all([
      page.waitForEvent('popup'),
      page.locator('text=Links /formpage/ >> a').click(),
    ]);

    await expect(page1.locator('.form__title')).toHaveText('Contact Us');
    await expect(page1).toHaveScreenshot({ maxDiffPixelRatio: 1, fullPage: true });
  });
});
