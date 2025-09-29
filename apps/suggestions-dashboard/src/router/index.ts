// import { createRouter, createWebHistory } from 'vue-router'

// const router = createRouter({
//   history: createWebHistory(import.meta.env.BASE_URL),
//   routes: [],
// })

// export default router


import { createRouter, createWebHistory } from 'vue-router';
import AppLayout from '@/views/layout/AppLayout.vue';

import { useAuthStore } from '@/stores/auth';
import employeeRoutes from '@/modules/employees/employeeRoutes';
import suggestionRoutes from '@/modules/suggestions/suggestionRoutes';
import AuthenticatedLayout from '@/views/layout/AuthenticatedLayout.vue';

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/login',
      name: 'login',
      component: () => import('@/modules/auth/views/LoginView.vue'),
    },
    {
      path: '/',
      component: AuthenticatedLayout,
      meta: { requiresAuth: true },
      redirect: '/dashboard',
      children: [
        {
          path: 'dashboard',
          name: 'dashboard',
          component: () => import('@/modules/dashboard/views/DashboardView.vue'),
        },
        ...employeeRoutes,
        ...suggestionRoutes
      ],
    },
  ],
});

// Navigation Guard
router.beforeEach((to, from, next) => {
  const authStore = useAuthStore();
  if (to.meta.requiresAuth && !authStore.state.isAuthenticated) {
    // Redirect to login if not authenticated
    next({ name: 'login' });
  } else {
    console.log('Navigation allowed to:', to.fullPath);
    next();
  }
});

export default router;