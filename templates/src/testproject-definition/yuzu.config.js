const yuzuHelpers = require('yuzu-definition-hbs-helpers');

module.exports = {
    blockDependenciesTimeout: 1000,
    hbsHelpers: yuzuHelpers,
    templatesRoot: '_templates',
    renderedPartialDirs: ['./_dev/_templates/blocks/', './_dev/_templates/_dataStructures/'],
    layoutDir: './_dev/_templates/_layouts/',
    registeredPartialsDirs: ['./_dev/_templates/blocks/'],
    dependantDirectories: ['./_dev/_templates/_layouts/', './_dev/_templates/blocks/'],
    templatePaths: '\\_client\\templatePaths.json',
    dist: {
        data: './_dev/_templates/**/*.json',
        schema: './_dev/_templates/**/*.schema',
        hbs: './_dev/_templates/**/*.hbs',
        markup: './_dev/_templates/**/*.html'
    },
    autoSchemaProperties : [
        {
            name: '_ref',
            schema: {
            "type": "string"
            }
        },
        {
            name: '_modifiers',
            schema: {
            "type": "string"
            }
        },
        {
            name: 'yuzu-path',
            schema: {
                "type": "string"
            }
        }
    ]
};