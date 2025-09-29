<script setup lang="ts">
import { ref } from 'vue';
import { useRouter } from 'vue-router';
import { useToast } from 'vue-toastification';
import { useAuthStore } from '@/stores/auth';
import { LocalAuthenticationService } from '../services/LocalAuthenticationService';

const username = ref('hsmanager@company.com');
const password = ref('password123');
const isLoading = ref(false);

const router = useRouter();
const toast = useToast();
const authStore = useAuthStore();
const authService = new LocalAuthenticationService();

const handleLogin = async () => {
  isLoading.value = true;
  try {
    const user = await authService.login(username.value, password.value);
    if (user) {
      authStore.setUsername(user.username);
      authStore.setId(user.id);
      toast.success(`Welcome back, ${user.username}!`);
      router.push({ name: 'dashboard' , replace: true });
    } else {
      toast.error('Invalid username or password.');
    }
  } catch (error) {
    toast.error('An unexpected error occurred.');
  } finally {
    isLoading.value = false;
  }
};
</script>
<template>
  <div class="container vh-100 d-flex justify-content-center align-items-center">
    <div class="card p-4" style="max-width: 400px; width: 100%;">
      <div class="card-body">
        <h3 class="card-title text-center mb-4">Login</h3>
        <form @submit.prevent="handleLogin">
          <div class="mb-3">
            <label for="username" class="form-label">Username or Email</label>
            <input type="text" class="form-control" id="username" v-model="username" required>
          </div>
          <div class="mb-3">
            <label for="password" class="form-label">Password</label>
            <input type="password" class="form-control" id="password" v-model="password" required>
          </div>
          <button type="submit" class="btn btn-primary w-100" :disabled="isLoading">
            <span v-if="isLoading" class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span>
            <span v-else>Login</span>
          </button>
        </form>
      </div>
    </div>
  </div>
</template>

<style scoped>
.content-container {
    min-height: 100vh;
    transition: margin-left 0.3s;
}
</style>