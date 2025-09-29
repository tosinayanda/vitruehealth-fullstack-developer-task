<script setup lang="ts">
defineProps<{
  title: string;
  value: number | string;
  icon: string;
  iconBgClass: string;
}>();
</script>
<template>
  <div class="card h-100">
    <div class="card-body d-flex align-items-center">
      <div class="flex-shrink-0 me-3">
        <div :class="iconBgClass" class="rounded-circle d-flex align-items-center justify-content-center" style="width: 48px; height: 48px;">
          <i :class="icon" class="fs-4 text-white"></i>
        </div>
      </div>
      <div class="flex-grow-1">
        <h6 class="text-muted mb-1">{{ title }}</h6>
        <h3 class="mb-0">{{ value }}</h3>
      </div>
    </div>
  </div>
</template>