// import { createApp } from 'vue'
// import App from './App.vue'
// import router from './router'

// const app = createApp(App)

// app.use(router)

// app.mount('#app')

// src/main.ts
import { createApp } from 'vue'

// Import Bootstrap
import "bootstrap/dist/css/bootstrap.min.css"
import "bootstrap-icons/font/bootstrap-icons.css"
import './assets/styles/theme.css'
// Import Toastification
import Toast, { type PluginOptions } from "vue-toastification";
import "vue-toastification/dist/index.css";

import App from './App.vue'
import router from './router'

import './assets/styles/theme.css' // Your custom styles

const app = createApp(App)

app.use(router)

// Configure Toastification
const options: PluginOptions = {
    transition: "Vue-Toastification__bounce",
    maxToasts: 5,
    newestOnTop: true
};
app.use(Toast, options);

app.mount('#app')

// Import Bootstrap JS at the end
import "bootstrap/dist/js/bootstrap.bundle.min.js"