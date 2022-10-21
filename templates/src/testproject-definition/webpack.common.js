const path = require('path');
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
    filename: './scripts/[name].js',
  },
  mode: 'none',
  resolve: {
    alias: {
      vueFiles: path.join(__dirname, '/_dev/_templates/blocks/'),
      libraryScripts: path.join(__dirname, '/_dev/_source/js/'),
      '@images': path.join(__dirname, '/_dev/_source/images/'),
    },
  },
  module: {
    rules: [{
        test: /\.(png|jpg|svg)$/,
        use: [{
          loader: 'file-loader',
          options: {
            name: '_client/images/[name].[ext]',
          }
        }]
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
  }
}
