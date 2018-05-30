const path = require('path')
const webpack = require('webpack')
const ExtractTextPlugin = require('extract-text-webpack-plugin')
const OptimizeCSSPlugin = require('optimize-css-assets-webpack-plugin')

module.exports = (env, argv) => {
  console.log('Building vendor', process.env.NODE_ENV)

  const isDevBuild = !(process.env.NODE_ENV && process.env.NODE_ENV === 'production')
  const extractCSS = new ExtractTextPlugin('vendor.css') // error extracting the bootstrap css text? (see logs)

  return [{
    // Redundant, but from Dotnet the mode cannot be set at command line.
    mode: argv && argv.mode ? argv.mode : (isDevBuild ? 'development' : 'production'),
    stats: { modules: false },
    resolve: {
      extensions: ['.js']
    },
    module: {
      rules: [
        { test: /\.(png|woff|woff2|eot|ttf|svg)(\?|$)/, use: 'url-loader?limit=100000' },
        { test: /\.css(\?|$)/, use: extractCSS.extract(['css-loader']) }
      ]
    },
    entry: {
      vendor: [
		  'bootstrap', 
		  'bootstrap/dist/css/bootstrap.css', 
		  'event-source-polyfill', 
		  'vue', 
		  'vuex', 
		  'axios', 
		  'vue-router', 
		  'jquery',
		  'bootstrap-vue',
		  'perfect-scrollbar',		  
		  '@coreui/coreui',
		  '@coreui/coreui-plugin-chartjs-custom-tooltips',
		  '@coreui/icons/css/coreui-icons.css', 
		  '@coreui/vue',
		  'vue-chartjs',
		  'chart.js',
		  'flag-icon-css/css/flag-icon.css'
	  ]
    },
    output: {
      path: path.join(__dirname, 'wwwroot', 'dist'),
      publicPath: '/dist/',
      filename: '[name].js',
      library: '[name]_[hash]'
    },
    plugins: [
      extractCSS,
      // Compress extracted CSS.
      new OptimizeCSSPlugin({
        cssProcessorOptions: {
          safe: true
        }
      }),
      new webpack.ProvidePlugin({
        $: 'jquery',
        jQuery: 'jquery',
        Popper: ['popper.js', 'default']
        /* For modal, you will need to add tether */
      }), // Maps these identifiers to the jQuery package (because Bootstrap expects it to be a global variable)
      new webpack.DllPlugin({
        path: path.join(__dirname, 'wwwroot', 'dist', '[name]-manifest.json'),
        name: '[name]_[hash]'
      }),
      new webpack.DefinePlugin({
        'process.env.NODE_ENV': isDevBuild ? '"development"' : '"production"'
      })
    ].concat(isDevBuild ? [] : [
      // new webpack.optimize.UglifyJsPlugin()
      // Replaced by a optimize option.
    ])
  }]
}
