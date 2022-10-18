const config = require('./webpack.common.config');
const yuzuApi = require('yuzu-definition-api');
const MiniCssExtractPlugin = require('mini-css-extract-plugin');
const webpack = require('webpack');
const yuzuPlugins = require('yuzu-definition-webpack-plugins');

config.mode = 'development';
config.devtool = 'source-map';
config.devServer = {
    index: 'yuzu.html',
    port: 3000,
    host: 'localhost',
    disableHostCheck: true,
    headers: {
        "Access-Control-Allow-Origin": "*",
        "Access-Control-Allow-Methods": "GET, POST, PUT, DELETE, PATCH, OPTIONS",
        "Access-Control-Allow-Headers": "X-Requested-With, content-type, Authorization"
    },
    watchOptions: {
        poll: true
    },
    before:(app) => {
        app.use('/api/', yuzuApi);
    }
};
config.plugins.push(
    new MiniCssExtractPlugin({
        filename: './_client/styles/[name].css'
    }),
    new webpack.NoEmitOnErrorsPlugin(),
    new yuzuPlugins.TemplatePaths()
);

config.entry.yuzu.push(`webpack-dev-server-status-bar`);

module.exports = config;