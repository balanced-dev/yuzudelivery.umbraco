import { test } from '@umbraco/playwright-testhelpers';
import { YuzuUiHelpers } from './YuzuUiHelpers';

const yuzuTest = test.extend({
  yuzuUi: async ({ page }, use) => {
    const yuzuUiHelpers = new YuzuUiHelpers(page);
    await use(yuzuUiHelpers);
  }
})

export { yuzuTest as test }