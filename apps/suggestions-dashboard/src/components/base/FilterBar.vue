<script setup lang="ts">
import { defineProps, defineEmits } from 'vue';

// --- Types for Reusability and Type Safety ---

interface FilterOption {
    label: string;
    value: string | number;
    // value: string | number | boolean;
}

interface FilterConfig {
    /** The key used to store this filter's value in the filter object (e.g., 'status') */
    key: string;
    /** The display name for the filter (e.g., 'Status') */
    label: string;
    /** The possible values for the dropdown */
    options: FilterOption[];
    /** Optional default value to use on reset (defaults to undefined/All) */
    defaultValue?: string | number;
}

// The filter object that contains the current state of all filters
interface FilterState {
    [key: string]: any; // Allows any key/value pair for the current filter values
    search: string; // Explicitly define the search key
}

// --- Props and Emits ---

const props = defineProps<{
    /** Configuration for the dynamic select filters */
    filters: FilterConfig[];
    /** The current filter state, used for v-model binding */
    modelValue: FilterState;
}>();

const emit = defineEmits<{
    /** Emitted when any filter changes, to update the parent's state */
    'update:modelValue': [value: FilterState];
}>();


// --- Methods ---

/**
 * Updates a specific filter key and emits the new, complete filter state.
 * @param key The key of the filter being updated ('search', 'status', etc.)
 * @param value The new value for that filter
 */
const updateFilter = (key: string, event: Event) => {
    const value = (event.target as HTMLSelectElement || event.target as HTMLInputElement).value; // Use HTMLSelectElement
    const newFilterState: FilterState = {
        ...props.modelValue,
        [key]: value
    };
    // Emit the full new state object
    emit('update:modelValue', newFilterState);
    console.log('Emitted update:modelValue with state:', newFilterState);
};

/**
 * Resets all filters to their initial defaults (undefined/empty string).
 */
const resetFilters = () => {
    const defaultState: FilterState = { search: '' };

    // Set default values for all dynamic select filters
    props.filters.forEach(filter => {
        defaultState[filter.key] = filter.defaultValue !== undefined ? filter.defaultValue : undefined;
    });

    emit('update:modelValue', defaultState);
    console.log('Emitted update:modelValue with reset state:', defaultState);
};
</script>

<template>
    <div class="row g-2 align-items-center mb-3">

        <div class="col-md-3">
            <input type="search" class="form-control" placeholder="Search..." :value="modelValue.search"
                @input="updateFilter('search', $event)" />
        </div>

        <div class="col" v-for="filter in filters" :key="filter.key">
            <label :for="filter.key">{{ filter.label }}</label>
            <select class="form-select" :value="modelValue[filter.key]" @change="updateFilter(filter.key, $event)">
                <option :value="undefined">{{ filter.label }}: All</option>

                <option v-for="option in filter.options" :key="option.value" :value="option.value">
                    {{ option.label }}
                </option>
            </select>
        </div>

        <div class="col-auto">
            <a href="#" @click.prevent="resetFilters">Reset</a>
        </div>
    </div>
</template>
