var container = document.createElement("DIV"),
  iframe = document.createElement("IFRAME"),
  resizeHandle = document.createElement("DIV"),
  settingsArea = document.createElement("DIV"),
  stylesheetLink = document.createElement("LINK"),
  yuzuWrapperClasses = {
    layout: "yuzu-layout-root",
    content: "yuzu-content-root",
  },
  errorMessagePrefix = "[YUZU-DEF-UI] - ",
  toggleClass = "yuzu-overlay--is-open",
  alignRightClass = "yuzu-overlay--dock-right",
  overlayCookieName = "yuzu-overlay-user-settings",
  isDragging = false,
  newOverlayWidth = 0,
  overlayResizeInertiaPx = 10,
  highlightBodyClass = "yuzu-highlight-active",
  highlightBlockClass = "yuzu-highlighted-block",
  iconSprite = document.createElement("DIV"),
  iconSpriteInnerHTML =
    '<svg style="border: 0;clip: rect(0 0 0 0); height: 1px; margin: -1px; overflow: hidden; padding: 0; position: absolute; width: 1px;" xmlns="http://www.w3.org/2000/svg"><defs><symbol id="yuzu-feather-settings" viewBox="0 0 24 24"><circle cx="12" cy="12" r="3"></circle><path d="M19.4 15a1.65 1.65 0 0 0 .33 1.82l.06.06a2 2 0 0 1 0 2.83 2 2 0 0 1-2.83 0l-.06-.06a1.65 1.65 0 0 0-1.82-.33 1.65 1.65 0 0 0-1 1.51V21a2 2 0 0 1-2 2 2 2 0 0 1-2-2v-.09A1.65 1.65 0 0 0 9 19.4a1.65 1.65 0 0 0-1.82.33l-.06.06a2 2 0 0 1-2.83 0 2 2 0 0 1 0-2.83l.06-.06a1.65 1.65 0 0 0 .33-1.82 1.65 1.65 0 0 0-1.51-1H3a2 2 0 0 1-2-2 2 2 0 0 1 2-2h.09A1.65 1.65 0 0 0 4.6 9a1.65 1.65 0 0 0-.33-1.82l-.06-.06a2 2 0 0 1 0-2.83 2 2 0 0 1 2.83 0l.06.06a1.65 1.65 0 0 0 1.82.33H9a1.65 1.65 0 0 0 1-1.51V3a2 2 0 0 1 2-2 2 2 0 0 1 2 2v.09a1.65 1.65 0 0 0 1 1.51 1.65 1.65 0 0 0 1.82-.33l.06-.06a2 2 0 0 1 2.83 0 2 2 0 0 1 0 2.83l-.06.06a1.65 1.65 0 0 0-.33 1.82V9a1.65 1.65 0 0 0 1.51 1H21a2 2 0 0 1 2 2 2 2 0 0 1-2 2h-.09a1.65 1.65 0 0 0-1.51 1z"></path></symbol><symbol id="yuzu-feather-maximize-2" viewBox="0 0 24 24"><polyline points="15 3 21 3 21 9"></polyline><polyline points="9 21 3 21 3 15"></polyline><line x1="21" y1="3" x2="14" y2="10"></line><line x1="3" y1="21" x2="10" y2="14"></line></symbol><symbol id="yuzu-feather-sidebar" viewBox="0 0 24 24"><rect x="3" y="3" width="18" height="18" rx="2" ry="2"></rect><line x1="9" y1="3" x2="9" y2="21"></line></symbol><symbol id="yuzu-feather-x" viewBox="0 0 24 24"><line x1="18" y1="6" x2="6" y2="18"></line><line x1="6" y1="6" x2="18" y2="18"></line></symbol><symbol id="yuzu-feather-menu" viewBox="0 0 24 24"><line x1="3" y1="12" x2="21" y2="12"></line><line x1="3" y1="6" x2="21" y2="6"></line><line x1="3" y1="18" x2="21" y2="18"></line></symbol>';

var buttons = {
  open: {
    addButtonClass: true,
    html:
      '<svg class="yuzu-overlay__button__icon yuzu-overlay__open__icon yuzu-feather-icon"><use xlink:href="#yuzu-feather-menu"></use></svg><span class="yuzu-overlay__button__text yuzu-overlay__open__text">Dev Tools</span>',
  },
  close: {
    addButtonClass: true,
    html:
      '<svg class="yuzu-overlay__button__icon yuzu-overlay__close__icon yuzu-feather-icon"><use xlink:href="#yuzu-feather-x"></use></svg><span class="yuzu-overlay__button__text yuzu-overlay__close__text">Close</span>',
  },
  settings: {
    addButtonClass: true,
    html:
      '<svg class="yuzu-overlay__button__icon yuzu-overlay__settings__icon yuzu-feather-icon"><use xlink:href="#yuzu-feather-settings"></use></svg><span class="yuzu-overlay__button__text yuzu-overlay__settings__text">Settings</span>',
  },
  fullscreen: {
    addButtonClass: true,
    html:
      '<svg class="yuzu-overlay__button__icon yuzu-overlay__fullscreen__icon yuzu-feather-icon"><use xlink:href="#yuzu-feather-maximize-2"></use></svg><span class="yuzu-overlay__button__text yuzu-overlay__fullscreen__text">Fullscreen</span>',
  },
  dock: {
    addButtonClass: true,
    html:
      '<svg class="yuzu-overlay__button__icon yuzu-overlay__dock__icon yuzu-feather-icon"><use xlink:href="#yuzu-feather-sidebar"></use></svg><span class="yuzu-overlay__button__text yuzu-overlay__dock__text">Change dock position</span>',
  },
};

var createButton = function (key) {
  var button = document.createElement("BUTTON"),
    settings = buttons[key];

  if (settings.addButtonClass) {
    button.classList.add("yuzu-overlay__button");
  }
  button.classList.add("yuzu-overlay__" + key);
  button.innerHTML = settings.html;
  return button;
};

var openButton = createButton("open"),
  closeButton = createButton("close"),
  settingsButton = createButton("settings"),
  fullscreenButton = createButton("fullscreen"),
  dockButton = createButton("dock");

var defaultOverlayUserSettings = {
  isOpen: false,
  overlayWidth: 300,
  isDockedRight: false,
};

var userSettings = {};

var randomIntFromInterval = function (
  min,
  max // min and max included
) {
  return Math.floor(Math.random() * (max - min + 1) + min);
};

var setupWs = function (wsId) {
  const url = "ws://localhost:8081/";
  const connection = new WebSocket(url);

  connection.onopen = function () {
    connection.send("setup:" + wsId);
  };

  connection.onerror = function (error) {
    console.log("WebSocket error: " + error);
  };

  connection.onmessage = function (e) {
    var response = JSON.parse(e.data);

    if (response.action == "preview") {
      if (response.data.indexOf(yuzuWrapperClasses.content) > -1) {
        var layoutRoot = document.querySelector(
            "." + yuzuWrapperClasses.layout
          ),
          responseLayoutRoot = document.createElement("div");

        responseLayoutRoot.innerHTML = response.data;

        var responseContent = responseLayoutRoot.querySelector(
          "." + yuzuWrapperClasses.layout
        );

        if (responseContent && layoutRoot) {
          layoutRoot.innerHTML = responseContent.innerHTML;
        } else {
          console.error(
            errorMessagePrefix +
              'Unable to find Yuzu layout wrapper (".' +
              yuzuWrapperClasses.layout +
              '") in document/response'
          );
        }

        responseLayoutRoot = null;
      } else {
        var contentWrapper = document.querySelector(
          "." + yuzuWrapperClasses.content
        );

        if (contentWrapper) {
          contentWrapper.innerHTML = response.data;
        } else {
          console.error(
            errorMessagePrefix +
              'Unable to find Yuzu content wrapper (".' +
              yuzuWrapperClasses.content +
              '") in document'
          );
        }
      }
      var refreshEvent = new Event("YUZU:CONTENT-REFRESH");
      document.dispatchEvent(refreshEvent);
    }
    if (response.action == "setActive") {
      var block = document.querySelector(
        "[data-yuzu='" + response.data.path + "']"
      );

      if (block && response.data.isActive === "true") {
        block.scrollIntoView({
          behavior: "smooth",
          block: "start",
        });
        setTimeout(() => {
          window.scrollBy(0, -50);
        }, 500);
        block.classList.add(highlightBlockClass);
        document.body.classList.add(highlightBodyClass);
      } else if (block) {
        block.classList.remove(highlightBlockClass);

        if (!document.querySelector(highlightBlockClass)) {
          document.body.classList.remove(highlightBodyClass);
        }
      } else {
        console.error("Yuzu-Def-UI: Unable to find block in markup");
      }
    }
  };

  window.onbeforeunload = function (event) {
    connection.close();
    createCookie(overlayCookieName, JSON.stringify(userSettings), 365);
  };
};

var createCookie = function (name, value, expires, path) {
  var cookieString = name + "=" + value + "; ";
  expiryDate = "";

  if (typeof expires === "number") {
    expiryDate = getDaysFromNow(expires);
    cookieString += "expires=" + expiryDate + "; ";
  }

  cookieString += path == undefined ? "path=/" : "path=" + path;
  document.cookie = cookieString;
};

var checkCookies = function (name) {
  var nameEQ = name + "=";
  var cookieArray = document.cookie.split(";");

  for (var i = 0; i < cookieArray.length; i++) {
    var c = cookieArray[i];

    // Remove spaces
    while (c.charAt(0) == " ") {
      c = c.substring(1, c.length);
    }

    // If cookie found
    if (c.indexOf(nameEQ) === 0) {
      return c.substring(nameEQ.length, c.length);
    }
  }
  return null;
};

var getDaysFromNow = function (days) {
  var date = new Date();
  date.setTime(date.getTime() + days * 24 * 60 * 60 * 1000);
  return date.toGMTString();
};

var toggleOverlay = function (e) {
  container.classList.toggle(toggleClass);
  userSettings.isOpen = !userSettings.isOpen;
};

var yuzuMouseMoveEvent = function (e) {
  newOverlayWidth = userSettings.isDockedRight
    ? window.innerWidth - e.clientX
    : e.clientX;

  if (
    isDragging ||
    Math.abs(userSettings.overlayWidth - newOverlayWidth) >
      overlayResizeInertiaPx
  ) {
    isDragging = true;
    container.classList.add("yuzu-overlay--is-dragging");
    container.style.width = newOverlayWidth + "px";
  }
};

var addResizeEvents = function () {
  resizeHandle.addEventListener("mousedown", function (e) {
    document.addEventListener("mousemove", yuzuMouseMoveEvent);
  });
  document.addEventListener("mouseup", function (e) {
    if (isDragging) {
      document.removeEventListener("mousemove", yuzuMouseMoveEvent);
      container.classList.remove("yuzu-overlay--is-dragging");
      userSettings.overlayWidth = newOverlayWidth;
      isDragging = false;
    }
  });
};

var addOverlayRepositionEvents = function () {
  dockButton.addEventListener("click", function (e) {
    container.classList.toggle(alignRightClass);
    userSettings.isDockedRight = !userSettings.isDockedRight;
  });
};

var addOverlayToggleEvents = function () {
  openButton.addEventListener("click", toggleOverlay);
  closeButton.addEventListener("click", toggleOverlay);
};

var setupHTML = function () {
  stylesheetLink.rel = "stylesheet";
  stylesheetLink.type = "text/css";
  stylesheetLink.href = "/yuzu-def-ui/overlay.css";

  container.classList.add("yuzu-overlay");
  container.style.width = userSettings.overlayWidth + "px";

  if (userSettings.isOpen) {
    container.classList.add(toggleClass);
  }
  if (userSettings.isDockedRight) {
    container.classList.add(alignRightClass);
  }

  resizeHandle.classList.add("yuzu-overlay__resize-handle");

  settingsArea.classList.add("yuzu-overlay__settings-area");

  iconSprite.innerHTML = iconSpriteInnerHTML;

  iframe.classList.add("yuzu-overlay__content");
  iframe.setAttribute("frameborder", "0");
  iframe.setAttribute("scrolling", "0");
  iframe.setAttribute("src", (src = "/yuzu-def-ui/index.html?wsId=" + wsId));
};

var initialiseSettings = function () {
  var cookieSettings = checkCookies(overlayCookieName);

  if (cookieSettings) {
    userSettings = JSON.parse(cookieSettings);
  } else {
    createCookie(
      overlayCookieName,
      JSON.stringify(defaultOverlayUserSettings),
      365
    );
    userSettings = defaultOverlayUserSettings;
  }
};

var addIframe = function (wsId) {
  initialiseSettings();
  setupHTML();

  container.appendChild(iframe);
  container.appendChild(resizeHandle);
  container.appendChild(stylesheetLink);
  container.appendChild(closeButton);
  container.appendChild(openButton);
  container.appendChild(settingsButton);
  container.appendChild(settingsArea);
  settingsArea.appendChild(dockButton);
  // settingsArea.appendChild(fullscreenButton);
  container.appendChild(iconSprite);
  document.getElementsByTagName("body")[0].appendChild(container);

  addResizeEvents();
  addOverlayToggleEvents();
  addOverlayRepositionEvents();
};

var wsId = randomIntFromInterval(100000, 999999);

module.exports = function () {
  
  setupWs(wsId);
  addIframe(wsId);

};
