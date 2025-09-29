<script setup lang="ts">
import { useAuthStore } from '@/stores/auth';
import { useRouter } from 'vue-router';

const authStore = useAuthStore();
const router = useRouter();

const handleLogout = () => {
  console.log('Logging out user:', authStore.state.username);
  authStore.logout();
  router.push({ name: 'login' , replace: true });
};
</script>

<template>
  <nav class="navbar navbar-expand-lg navbar-light bg-light border-bottom">
    <div class="container-fluid">
      <button class="btn btn-primary d-md-none" type="button" data-bs-toggle="offcanvas" data-bs-target="#sidebarOffcanvas" aria-controls="sidebarOffcanvas">
        <i class="bi bi-list"></i>
      </button>

      <div class="ms-auto d-flex align-items-center">
        <div class="dropdown">
          <a href="#" class="d-block link-dark text-decoration-none dropdown-toggle" id="dropdownUser1" data-bs-toggle="dropdown" aria-expanded="false">
            <i class="bi bi-person-circle fs-4 me-2"></i>
            <span class="d-none d-sm-inline">{{ authStore.state.username }}</span>
          </a>
          <ul class="dropdown-menu dropdown-menu-end text-small" aria-labelledby="dropdownUser1">
            <li><a class="dropdown-item" href="#">Profile</a></li>
            <li><hr class="dropdown-divider"></li>
            <li><button class="dropdown-item" @click="handleLogout">Sign out</button></li>
          </ul>
        </div>
      </div>
    </div>
  </nav>
</template>