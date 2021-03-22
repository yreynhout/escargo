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
      minHeight: {
        "320": "320px"
      },
      width: {
        "320": "320px"
      },
      colors: {
        "primary": "#dcd2ae"
      },
      backgroundColor: {
        "primary": "#dcd2ae"
      },
      backgroundImage: {
        "composition8": "url('../images/composition8.jpg')"
      },
    }
  },
  variants: {
    extend: {
      opacity: ['disabled'],
    },
  },
  plugins: [
    require('@tailwindcss/forms'),
  ]
}