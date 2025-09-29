<script setup lang="ts" generic="T extends Record<string, any>">
import { computed } from 'vue';

type CustomKeys = 'actions' | 'status' | '#'; // Add any other slot-only keys

interface Column {
  key: string;
  // key: keyof T | CustomKeys;
  label: string;
  class?: string;
}

const props = defineProps<{
  columns: Column[];
  items: T[];
  isSelectable?: boolean;
}>();

const selectedItems = defineModel<T[]>('selectedItems');

// Handle master checkbox
const allSelected = computed({
  get: () => props.items.length > 0 && selectedItems.value?.length === props.items.length,
  set: (value) => {
    selectedItems.value = value ? [...props.items] : [];
  }
});
</script>

<template>
  <div class="table-responsive">
    <table class="table table-hover align-middle">
      <thead>
        <tr>
          <th v-if="isSelectable">
            <input class="form-check-input" type="checkbox" v-model="allSelected" />
          </th>
          <th v-for="col in columns" :key="String(col.key)" :class="col.class">{{ col.label }}</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="item in items" :key="item.id">
          <td v-if="isSelectable">
            <input class="form-check-input" type="checkbox" :value="item" v-model="selectedItems" />
          </td>
          <td v-for="col in columns" :key="`${item.id}-${String(col.key)}`">
            <slot :name="`cell(${String(col.key)})`" :item="item">
              {{ item[col.key] }}
            </slot>
          </td>
        </tr>
        <tr v-if="items.length === 0">
          <td :colspan="columns.length + (isSelectable ? 1 : 0)" class="text-center">
            No data available.
          </td>
        </tr>
      </tbody>
    </table>
  </div>
</template>