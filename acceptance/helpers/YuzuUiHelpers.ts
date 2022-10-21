import { Page } from '@playwright/test';

class YuzuUiHelpers {
  page: Page;

  constructor(page: Page) {
    this.page = page;
  }

  async getTreeItem(treeName, itemNamePathArray, options = null) {
    await this.page.locator('li > .umb-tree-root a[href*=' + treeName + ']').first().isVisible();

    let finalLocator = await this.page
      .locator('li > .umb-tree-root a[href*=' + treeName + ']')
      .locator("xpath=ancestor::li");

    for (let i = 0; i < itemNamePathArray.length; i++) {
      // We want to find the outer li of the tree item, to be able to search deeper, there may be multiple results
      // for multiple levels of nesting, but it should be okay to pick last, since that should pick the immediate parent
      // the search goes from outer most to inner most (I think)
      finalLocator = await finalLocator
        .locator('.umb-tree-item__label >> text=' + itemNamePathArray[i])
        .locator('xpath=ancestor::li[contains(@class, "umb-tree-item")]')
        .last();

      // We don't want to click the expand option if we're on the last item, 
      // since that can cause issues when clicking the "root" items, such as Document Types.
      if (i + 1 == itemNamePathArray.length) {
        break;
      }

      // Get the UL with the collapsed state, if it exists
      const ulObject = await finalLocator.locator(".collapsed");
      const locatorIcon = await finalLocator.locator('[data-element="tree-item-expand"]', { hasText: itemNamePathArray[i] }).innerHTML();

      // Check if an element is actually expanded, if not expanded, it will have the "icon-navigation-right"
      if (locatorIcon.includes("icon-navigation-right")) {
        if (await ulObject.count() > 0) {
          // Get the expand button
          const expandButton = finalLocator.locator('[data-element="tree-item-expand"]', { hasText: itemNamePathArray[i] });
          // Weirdly if a tree item has no children is still marked as expanded, however, the expand button is hidden
          // So we have to ensure that the button is not hidden before we click.
          if (await expandButton.isHidden() === false) {
            // Click the expand button, if its collapsed
            await finalLocator.locator('[data-element="tree-item-expand"]', { hasText: itemNamePathArray[i] }).click(options);
          }
        }
      }
    }
    return finalLocator;
  }
}

export { YuzuUiHelpers };
