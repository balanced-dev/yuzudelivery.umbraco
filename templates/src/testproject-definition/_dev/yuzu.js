require('./yuzu-def-ui/websocketClient.js')();

const urlParams = new URLSearchParams(window.location.search);

const type = urlParams.get('type');
const name = urlParams.get('name');

const prefix = type == 'blocks' ? 'par' : '';

const area = urlParams.get('area') ? urlParams.get('area') : '';
const state = urlParams.get('state') ? urlParams.get('state') : '';

const writeFunc = (output) => {
  const body = document.body;

  let temp = document.createElement('div');
  temp.innerHTML = output.default;
  let html = temp.firstChild;

  body.prepend(html);
  const layoutRoot = document.querySelector('.yuzu-layout-root');
  const htmlClasses = layoutRoot.dataset.htmlClasses;
  if (htmlClasses) {
    htmlClasses.split(' ').forEach(cssClass => {
      document.documentElement.classList.add(cssClass);
    });
  }
}

const lowerFirst = (input) => {
  if (!input) {
    return input;
  }

  return input.charAt(0).toLowerCase() + input.slice(1);
}

if (name && type) {
  const filename = `${prefix}${name}${state ? '_' + state : ''}`;

  if (area) {
    // https://webpack.js.org/api/module-methods/#dynamic-expressions-in-import
    import(`./_templates/${type}/${area}/${lowerFirst(name)}/data/${filename}.json`)
      .then(writeFunc);
  } else {
    import(`./_templates/${type}/${lowerFirst(name)}/data/${filename}.json`)
      .then(writeFunc);
  }
}
