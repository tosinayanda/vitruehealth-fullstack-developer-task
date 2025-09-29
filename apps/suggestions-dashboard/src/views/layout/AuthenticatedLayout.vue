<script setup lang="ts">
import SideBar from '@/components/layout/SideBar.vue';
import TopBar from '@/components/layout/TopBar.vue';
import { useAuthStore } from '@/stores/auth';
import { computed } from 'vue'; // 💡 Import computed

const authStore = useAuthStore();
const isAuthenticated = computed(() => authStore.state.isAuthenticated); 

</script>

<template>
    <div class="d-flex">
        <SideBar v-if="isAuthenticated" />
        <div class="content-container flex-grow-1">
            <TopBar v-if="isAuthenticated" />
            <main class="main-content m-1">
                <RouterView />
            </main>
        </div>
    </div>
</template>

<style scoped>
.content-container {
    min-height: 100vh;
    transition: margin-left 0.3s;
}
</style>