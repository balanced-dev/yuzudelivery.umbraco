const config = require('./webpack.common.config');
const MiniCssExtractPlugin = require('mini-css-extract-plugin');

const webpack = require('webpack');
const yuzuPlugins = require('yuzu-definition-webpack-plugins');

config.mode = 'production';

config.output.filename = './_client/script/[name].[contenthash].js';
config.plugins.push(
    new MiniCssExtractPlugin({
        filename: './_client/style/[name].[contenthash].css'
    }),
    new MiniCssExtractPlugin({
        filename: './_client/style/[name].css'
    }),
    new yuzuPlugins.TemplatePaths('yuzu.html'),
    new yuzuPlugins.Dist(),
    new webpack.DefinePlugin({
        'process.env.NODE_ENV': JSON.stringify('production')
    })
);

module.exports = config;