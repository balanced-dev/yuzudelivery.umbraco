const path = require('path');
const { CleanWebpackPlugin } = require('clean-webpack-plugin');
const HtmlWepackPlugin = require('html-webpack-plugin');

const MiniCssExtractPlugin = require('mini-css-extract-plugin');

const CopyPlugin = require('copy-webpack-plugin');
const globImporter = require('node-sass-glob-importer');

files = {
    templates: './_dev/_templates/',
    templateHTML: './_dev/_templates/'
}

paths = {
    images: {
        dest: './_dev/_source/images/',
    }
}

module.exports = {
    entry: {
        'yuzu': ['./_dev/yuzu.js'], 
        'scripts': './_dev/_source/js/scripts.js',
        'styles': './_dev/_source/styles/scss/frontend.js',
        'backoffice': './_dev/_source/styles/scss/backoffice.js'
    },
    output: {
        filename: './_client/scripts/[name].js',
        path: path.resolve(__dirname, './dist/'),
        publicPath: '',        
    },
    mode: 'none',
    resolve: {
        alias: {
            vueFiles: path.join(__dirname, '/_dev/_templates/blocks/'),
            libraryScripts: path.join(__dirname, '/_dev/_source/js/'),
        },
    },
    module: {
        rules: [
            {
                test: /\.(png|jpg|svg)$/,
                use: [
                    { 
                        loader: 'file-loader',
                        options: {
                            name: '_client/images/[name].[ext]',
                        }
                    }
                ]
            },
            {
                test: /\.scss$/,
                use: [
                    {   
                        loader: MiniCssExtractPlugin.loader,
                        options: {
                            publicPath: '../../'
                        }
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
            },
            {
                type: 'javascript/auto',
                test: /\.json$/,
                use: [
                    'html-loader',
                    'yuzu-loader'
                ]
            },
            {
                test: /\.html$/,
                use: [
                    'html-loader'
                ]
            },
            {
                test: /\.m?js$/,
                exclude: /node_modules/,
                use: {
                  loader: "babel-loader",
                  options: {
                    presets: ['@babel/preset-env']
                  }
                }
            }
        ]
    }, 
    plugins: [
        new CleanWebpackPlugin(),
        new HtmlWepackPlugin({
            title: 'Yuzu Pattern Library',
            chunks: ['scripts', 'styles', 'yuzu'],
            template: './_dev/templates.html',
            filename: 'yuzu.html'
        }),
        new CopyPlugin({
            patterns: [
                {
                    context: path.resolve(__dirname, '_dev', 'yuzu-def-ui'),
                    from: '**/*',
                    to: 'yuzu-def-ui'
                }
            ]
        }),
        new CopyPlugin({
            patterns: [
                {
                    context: path.resolve(__dirname, '_dev', '_source', 'images'),
                    from: '**/*',
                    to: path.resolve(__dirname, 'dist', '_client', 'images')
                }
            ]
        })
    ]
}