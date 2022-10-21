const { merge } = require('webpack-merge');
const common = require('./webpack.common');
const path = require('path');
const MiniCssExtractPlugin = require('mini-css-extract-plugin');
const globImporter = require('node-sass-glob-importer');
const webpack = require('webpack');
const yuzuPlugins = require('yuzu-definition-webpack-plugins');
const CopyPlugin = require('copy-webpack-plugin');
const { CleanWebpackPlugin } = require('clean-webpack-plugin');
const HtmlWepackPlugin = require('html-webpack-plugin');
const glob = require('glob');

function addLayoutTemplateFilePlugins(layoutsPath, plugins) {
  const templates = glob.sync(`${layoutsPath}/**/*.template.cshtml`)

  templates.forEach(template => {
    const layoutName = path.basename(template).replace('.template.', '.');
    plugins.push(
      new HtmlWepackPlugin({
        chunks: ['scripts', 'styles'],
        template: template,
        filename: `${layoutsPath}/${layoutName}`,
        publicPath: '/',
        minify: false
      })
    )
  });
}

module.exports = env => {
  const outputFolder = env && env.output ?
    path.resolve(__dirname, env.output, 'wwwroot') :
    path.resolve(__dirname, 'dist', 'wwwroot');

  const conditionals = {
    plugins: []
  };

  if (env && env.output) {
    conditionals.plugins.push(
      new CleanWebpackPlugin({
        cleanOnceBeforeBuildPatterns: [
          '../Yuzu/_templates/**/*',
          '_client/**/*',
          'yuzu-def-ui/**/*'
        ],
        dangerouslyAllowCleanPatternsOutsideProject: true,
        dry: false
      })
    );
  } else {
    conditionals.plugins.push(
      new CleanWebpackPlugin()
    );
  }

  addLayoutTemplateFilePlugins(path.resolve(outputFolder, '..', 'Views', 'Shared'), conditionals.plugins);

  return merge(common, conditionals, {
    mode: 'production',
    devtool: 'source-map',
    output: {
      filename: '_client/scripts/[name].[contenthash].js',
      path: outputFolder,
    },
    module: {
      rules: [{
        test: /\.scss$/,
        use: [{
            loader: MiniCssExtractPlugin.loader,
            options: {
              publicPath: '/',
            },
          },
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
      new MiniCssExtractPlugin({
        filename: '_client/style/[name].[contenthash].css'
      }),
      new CopyPlugin({
        patterns: [{
          context: path.resolve(__dirname, '_dev', '_source', 'images'),
          from: '**/*',
          to: path.resolve(outputFolder, '_client', 'images')
        }]
      }),
      new CopyPlugin({
        patterns: [{
          context: path.resolve(__dirname, '_dev', 'yuzu-def-ui'),
          from: '**/*',
          to: path.resolve(outputFolder, 'yuzu-def-ui')
        }]
      }),
      new HtmlWepackPlugin({
        title: 'Yuzu Pattern Library',
        chunks: ['scripts', 'styles', 'yuzu'],
        template: './_dev/index.template.html',
        filename: 'yuzu.html',
        publicPath: '/',
        minify: false
      }),
      new yuzuPlugins.TemplatePaths(),
      new yuzuPlugins.Dist({
        outputFolderRoot: '../Yuzu/_templates/'
      }),
    ]
  });
}
