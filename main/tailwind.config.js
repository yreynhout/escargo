module.exports = {
  purge: {
    content: [
      ".dist/*.html"
    ]
  },
  darkMode: false, // or "media' or 'class'
  theme: {
    extend: {
      minWidth: {
        "320": "320px"
      },
      width: {
        "320": "320px"
      },
      backgroundImage: {
        "composition8": "url('../images/composition8.jpg')"
      },
    }
  },
  variants: {
    extend: {},
  },
  plugins: []
}