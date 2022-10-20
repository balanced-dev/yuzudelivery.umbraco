const fs = require('fs');

const configPath = './.env'

console.log("Setting up config")

var fileContent = `UMBRACO_USER_LOGIN=e2e@hifi.agency
UMBRACO_USER_PASSWORD=TestThis42!
URL=http://localhost:8080`;

if(process.argv.slice(2) == 'ci') {
    console.log("Adding CI switch")
    fileContent = fileContent +'\nCI=true';
    console.log(fileContent)
}

fs.writeFile(configPath, fileContent, function (error) {
    if (error) return console.error(error);
    console.log('Configuration saved');
});