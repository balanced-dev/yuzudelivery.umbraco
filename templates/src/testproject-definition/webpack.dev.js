const { merge } = require('webpack-merge');
const common = require('./webpack.common');
const path = require('path');
const yuzuApi = require('yuzu-definition-api');
const globImporter = require('node-sass-glob-importer');
const webpack = require('webpack');
const yuzuPlugins = require('yuzu-definition-webpack-plugins');
const CopyPlugin = require('copy-webpack-plugin');
const HtmlWepackPlugin = require('html-webpack-plugin');

module.exports = merge(common, {
  mode: 'development',
  devtool: 'inline-source-map',
  devServer: {
    host: 'localhost',
    port: 3000,
    hot: true,
    disableHostCheck: true,
    headers: {
      "Access-Control-Allow-Origin": "*",
      "Access-Control-Allow-Methods": "GET, POST, PUT, DELETE, PATCH, OPTIONS",
      "Access-Control-Allow-Headers": "X-Requested-With, content-type, Authorization"
    },
    watchOptions: {
      poll: true
    },
    before: (app) => {
      app.use('/api/', yuzuApi);
    }
  },
  entry: {
    yuzu: ['webpack-dev-server-status-bar']
  },
  module: {
    rules: [{
      test: /\.scss$/,
      use: [
        'style-loader',
        'css-loader',
        'postcss-loader',
        {
          loader: 'sass-loader',
          options: {
            sassOptions: {
              importer: globImporter()
            }
          }
        }
      ]
    }]
  },
  plugins: [
    new webpack.NoEmitOnErrorsPlugin(),
    new yuzuPlugins.TemplatePaths({
      rootPath: ''
    }),
    new CopyPlugin({
      patterns: [{
        context: path.resolve(__dirname, '_dev', 'yuzu-def-ui'),
        from: '**/*',
        to: 'yuzu-def-ui'
      }]
    }),
    new HtmlWepackPlugin({
      title: 'Yuzu Pattern Library',
      chunks: ['scripts', 'styles', 'yuzu'],
      template: './_dev/index.template.html'
    })
  ]
});
