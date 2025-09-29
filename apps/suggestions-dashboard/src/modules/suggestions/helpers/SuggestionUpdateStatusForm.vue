<script setup lang="ts">
import { ref, computed, watch } from 'vue';
import { SuggestionService } from '../services/SuggestionService';
import { useAuthStore } from '@/stores/auth';
import { useToast } from 'vue-toastification';

const authStore = useAuthStore();
const toast = useToast();

const props = defineProps<{
    initialRecords: any[];
}>();

const emit = defineEmits(['submitted']);

const suggestionService = new SuggestionService();

// --- State ---
const localRecords = ref([...props.initialRecords]); // Mutable copy of records
const selectedStatus = ref('');
const validationErrors = ref<number[]>([]); // Stores IDs of records that failed validation

// --- Validation Logic ---

/**
 * Custom validation function: Prevents changing status from 'Completed' to 'InProgress'.
 */
const isValidTransition = (recordStatus: string, newStatus: string): boolean => {
    if (recordStatus === 'Completed' && newStatus !== 'Completed' || recordStatus === 'Dismissed' && newStatus !== 'Dismissed') {
        return false;
    }
    console.log('Valid transition from', recordStatus, 'to', newStatus);
    return true;
};

const runValidation = () => {
    if (!selectedStatus.value) {
        validationErrors.value = [];
        return;
    }

    const errors: number[] = [];
    localRecords.value.forEach(record => {
        if (!isValidTransition(record.status, selectedStatus.value)) {
            errors.push(record.id);
        }
    });
    validationErrors.value = errors;
};

// --- Computed Properties ---

const hasErrors = computed(() => validationErrors.value.length > 0);

// Records that are valid for the selected status
const validRecords = computed(() => {
    return localRecords.value.filter(record => !validationErrors.value.includes(record.id));
});

// Enable submit button only if a status is selected AND there are valid records
const isSubmitEnabled = computed(() => {
    return !!selectedStatus.value && validRecords.value.length > 0;
});

// --- Methods ---

const removeRecord = (id: number) => {
    localRecords.value = localRecords.value.filter(record => record.id !== id);
    // Re-run validation after removing a record
    runValidation();
};

const handleSubmit = async () => {
    if (!isSubmitEnabled.value) return;
    
    const updatePayload = validRecords.value.map(record => ({ ...record, status: selectedStatus.value, createdByAdminId: authStore.state.id }));

    try {
        console.log('API call Request Payload:', updatePayload);

        var response = await suggestionService.updateSuggestionsBulk(updatePayload);
        console.log('API call Response :', response);

        emit('submitted', validRecords.value);
        toast.success('Completed status update for suggestions successfully')
    } catch (error) {
        console.error('Update failed:', error);
         toast.error('Unable to complete status update successfully.');
        // Display user-friendly error toast
    }
};

// --- Watcher to trigger validation when records change (e.g., one is removed) ---
watch(localRecords, runValidation, { deep: true });
// Run validation on initial load if a status is pre-selected
if (selectedStatus.value) runValidation();

</script>

<template>
    <div>
        <h6 class="mb-2">Selected Records ({{ validRecords.length }} valid)</h6>
        <div class="d-flex flex-wrap gap-2 mb-4 p-3 bg-light rounded">
            <span v-for="record in localRecords" :key="record.id" :class="['badge', 'p-2', 'd-flex', 'align-items-center',
                validationErrors.includes(record.id) ? 'bg-danger' : 'bg-primary']">
                {{ record.name || record.id }} ({{ record.status }})
                <button type="button" class="btn-close btn-close-white ms-2" @click="removeRecord(record.id)"
                    aria-label="Remove record"></button>
            </span>
            <p v-if="!localRecords.length" class="text-muted mb-0">No records selected for update.</p>
        </div>

        <div class="mb-3">
            <label class="form-label">Set New Status</label>
            <select class="form-select" v-model="selectedStatus" @change="runValidation">
                <option value="">-- Select a Status --</option>
                <option value="Pending">Pending</option>
                <option value="In_Progress">In Progress</option>
                <option value="Overdue">Overdue</option>
                <option value="Completed">Completed</option>
                <option value="Dismissed">Dismissed</option>
            </select>
        </div>

        <div v-if="hasErrors" class="alert alert-danger">
            Validation failed for {{ validationErrors.length }} record(s). They are highlighted in red. Please remove
            them or select a different status.
        </div>

        <div class="d-flex justify-content-end mt-4">
            <button type="button" class="btn btn-success" :disabled="!isSubmitEnabled" @click="handleSubmit">
                Submit Update ({{ validRecords.length }})
            </button>
        </div>
    </div>
</template>
