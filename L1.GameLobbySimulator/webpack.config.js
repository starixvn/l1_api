const path = require("path");

module.exports = {
  entry: "./views/index.jsx",
  output: {
    path: path.join(__dirname, "public"),
    filename: "bundle.js"
  },
  module: {
    rules: [
      {
        test: /\.jsx?$/,
        exclude: /node_modules/,
        use: {
          query: {
            presets: ["@babel/preset-env", "@babel/preset-react"]
          },
          loader: "babel-loader"
        }
      },
      {
        test: /\.s?[ac]ss$/,
        use: ["style-loader", "css-loader", "sass-loader"]
      }
    ]
  }
};
