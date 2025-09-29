<template>
    <div class="modal fade show d-block" tabindex="-1" @click.self="$emit('close')">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Create Suggestion</h5>
                    <button type="button" class="btn-close" @click="$emit('close')"></button>
                </div>
                <div class="modal-body">
                    <SuggestionCreateForm :initial-records="records" @submitted="handleSubmitted" />
                </div>
            </div>
        </div>
    </div>
    <div class="modal-backdrop fade show"></div>
</template>

<script setup lang="ts">
import { defineProps, defineEmits } from 'vue';
import SuggestionCreateForm from '../helpers/SuggestionCreateForm.vue';

const props = defineProps<{
    records: any[]; // The list of records to be updated
}>();

const emit = defineEmits(['close', 'success']);

// Pass the submitted event data up to the table container
const handleSubmitted = (updatedRecords: any[]) => {
    emit('success', updatedRecords);
};
</script>

<style scoped>
/* Basic styling to make the modal display */
.modal {
    background: rgba(0, 0, 0, 0.5);
}
</style>