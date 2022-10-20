import {ConstantHelper, test} from '@umbraco/playwright-testhelpers';

import { expect } from '@playwright/test';

import { UmbracoMarketing } from '../pages/umbraco-marketing.page';
import { UmbracoForms } from '../pages/umbraco-forms.page';

test.describe('Content tests', () => {

    test.beforeEach(async ({page, umbracoApi}) => {
      await page.waitForTimeout(1000);
      await umbracoApi.login(true);
    });


    test('formpage renders correctly', async ({ page, umbracoUi }) => {

      const umbracoMarketing = new UmbracoMarketing(page);
      const umbrcoForms = new UmbracoForms(page);

      await umbracoMarketing.clear();

      await umbrcoForms.createContactForm();

      await umbracoUi.goToSection(ConstantHelper.sections.content);
      await umbracoUi.clickElement(umbracoUi.getTreeItem('content', ["Home", "FormPage"]));

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
      await expect(page1).toHaveScreenshot();


    });

  });
