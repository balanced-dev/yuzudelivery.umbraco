require('./yuzu-def-ui/websocketClient.js')();

const urlParams = new URLSearchParams(window.location.search);
const type = urlParams.get('type');
const area = urlParams.get('area') ? urlParams.get('area') : '';
const prefix = type == 'blocks' ? 'par' : '';
const name = urlParams.get('name');
const state = urlParams.get('state') ? urlParams.get('state') : '';

const writeFunc = (output) => {

  // const body = document.querySelector('div');
  // body.innerHTML = output.default;

  const body = document.body;
  const bodyConents = body.innerHTML;
  let temp = document.createElement('div');
  temp.innerHTML = output.default;
  let html = temp.firstChild;
  
  body.prepend(html);
  const layoutRoot = document.querySelector('.yuzu-layout-root');
  const htmlClasses = layoutRoot.dataset.htmlClasses;
  if(htmlClasses) {
    htmlClasses.split(' ').forEach(cssClass => {
      document.documentElement.classList.add(cssClass);
    });
  }
}

const lowerFirst = (string) => {
  return string.charAt(0).toLowerCase() + string.slice(1);
}

if(!area) {

  import(`../_dev/_templates/${type}/${lowerFirst(name)}/data/${prefix}${name}${state ? '_' + state : ''}.json`)
    .then(writeFunc);
}
else {

  import(`../_dev/_templates/${type}/${area}/${lowerFirst(name)}/data/${prefix}${name}${state ? '_' + state : ''}.json`)
    .then(writeFunc);
}
