const colors = require('tailwindcss/colors');

module.exports = {
    content: ['./src/**/*.{html,ts}'],
    darkMode: false, // or 'media' or 'class'
    theme: {
        fontFamily: {
            sans: ['Roboto', 'ui-sans-serif', 'system-ui', '-apple-system', 'BlinkMacSystemFont', 'Helvetica Neue', 'Arial', 'Noto Sans', 'sans-serif'],
        },
        screens: {
            sm: '576px',
            md: '768px',
            lg: '992px',
            xl: '1200px',
            '2xl': '1400px',
        },
        colors: {
            transparent: 'transparent',
            current: 'currentColor',
            black: colors.black,
            white: colors.white,
            gray: colors.gray,
            red: colors.red,
            yellow: colors.yellow,
            green: colors.green,
            blue: {
                50: 'var(--color-blue-50)',
                100: 'var(--color-blue-100)',
                200: 'var(--color-blue-200)',
                300: 'var(--color-blue-300)',
                400: 'var(--color-blue-400)',
                500: 'var(--color-blue-500)',
                600: 'var(--color-blue-600)',
                700: 'var(--color-blue-700)',
                800: 'var(--color-blue-800)',
                900: 'var(--color-blue-900)',
                950: 'var(--color-blue-950)',
            },
            primary: 'var(--color-primary)',
            secondary: 'var(--color-secondary)',
            success: 'var(--color-success)',
            warning: 'var(--color-warning)',
            danger: 'var(--color-danger)',
        },
        extend: {},
    },
    variants: {
        extend: {},
    },
    plugins: [require('@tailwindcss/forms')],
};
