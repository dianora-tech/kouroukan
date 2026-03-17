export default defineAppConfig({
  ui: {
    colors: {
      primary: 'green',
      secondary: 'red',
      neutral: 'slate',
    },
    button: {
      defaultVariants: {
        size: 'md',
      },
    },
    card: {
      slots: {
        root: 'rounded-lg',
      },
    },
  },
})
