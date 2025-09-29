<script setup lang="ts">
import { ref, computed, watch, onMounted } from 'vue';
import { SuggestionService } from '../services/SuggestionService';
import { EmployeeService } from '@/modules/employees/services/EmployeeService';
import { useAuthStore } from '@/stores/auth';
import { RiskLevel, Source, SuggestionStatus, SuggestionType, type Suggestion } from '@/types/Suggestions';
import { useToast } from 'vue-toastification';

const authStore = useAuthStore();
const toast = useToast();

const props = defineProps<{
    initialRecords: any[];
}>();

const emit = defineEmits(['submitted']);

const suggestionService = new SuggestionService();
const employeeService = new EmployeeService();

// --- State ---
const localRecords = ref([...props.initialRecords]); // Mutable copy of records
const selectedStatus = ref('');
const selectedPriority = ref('Low');
const selectedType = ref('Exercise');
const selectedRiskLevel = ref('Low');
const Description = ref('');
const Notes = ref('');
const validationErrors = ref<number[]>([]); // Stores IDs of records that failed validation

// --- Validation Logic ---

/**
 * Custom validation function: Prevents changing status from 'Completed' to 'InProgress'.
 */
const isValidTransition = (recordStatus: string, newStatus: string): boolean => {
    if (recordStatus === 'Completed' && newStatus === 'InProgress') {
        return false;
    }
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
    return validRecords.value.length > 0;
});

// --- Methods ---

const removeRecord = (id: number) => {
    localRecords.value = localRecords.value.filter(record => record.id !== id);
    // Re-run validation after removing a record
    runValidation();
};

const handleSubmit = async () => {
    if (!isSubmitEnabled.value) return;

    console.log('Submitting suggestions for records:', validRecords.value);

    console.log('localRecords:', localRecords.value);

    const updatePayload: Suggestion[] = validRecords.value.map(record => ({
        id: null,
        name: '',
        status: SuggestionStatus.Pending,
        employeeId: record?.id ?? record,
        description: Description.value,
        // riskLevel : selectedRiskLevel.value,
        notes: Notes.value,
        source: Source.Admin,
        priority: selectedPriority.value,
        type: selectedType.value,
        createdByAdminId: authStore.state.id,
        dateCreated: new Date(),
        dateUpdated: new Date(),
    }));

    try {

        console.log('API call RequestPayload:', updatePayload);
        const response = await suggestionService.createSuggestionsBulk(updatePayload);
        console.log('API call Response: Success');
        toast.success('Completed creation of suggestions successfully')

        // 3. Emit success event with the records that were updated
        emit('submitted', validRecords.value);

    } catch (error) {
        console.error('Update failed:', error);
        // Display user-friendly error toast
        toast.error('Unable to complete suggestion creation successfully');
    }
};

const employees = ref<any[]>([]);
const loadEmployees = async () => {
    try {
        const response = await employeeService.getAllEmployees(1, 10000);
        employees.value = response.data ?? [];
    } catch (error) {
        console.error('Failed to load employees:', error);
    }
};

// --- Watcher to trigger validation when records change (e.g., one is removed) ---
watch(localRecords, runValidation, { deep: true });
// Run validation on initial load if a status is pre-selected
if (selectedStatus.value) runValidation();

// --- Lifecycle Hook: Initial Data Load ---
onMounted(() => {
    loadEmployees();
});

</script>

<template>
    <div>
        <h6 class="mb-2">Selected Employees ({{ validRecords.length }} valid)</h6>
        <div class="d-flex flex-wrap gap-2 mb-4 p-3 bg-light rounded">
            <p v-if="!localRecords.length" class="text-muted mb-0">No employees selected for suggestion creation</p>
        </div>

        <div class="mb-3">
            <label class="form-label">Select Employees</label>
            <select class="form-select" multiple aria-label="multiple select example" v-model="localRecords"
                @change="runValidation">
                <option v-for="employee in employees" :key="employee.id" :value="employee.id">
                    {{ employee.name }}
                </option>
            </select>
        </div>

        <!-- <div class="mb-3">
            <label class="form-label">Set RiskLevel</label>
            <select class="form-select" v-model="selectedRiskLevel">
                <option value="Low" selected>Risk</option>
                <option value="Medium">Medium</option>
                <option value="High">High</option>
            </select>
        </div> -->

        <div class="mb-3">
            <label class="form-label">Set Priority</label>
            <select class="form-select" v-model="selectedPriority">
                <option value="Low" selected>Low</option>
                <option value="Medium">Medium</option>
                <option value="High">High</option>
            </select>
        </div>

        <div class="mb-3">
            <label class="form-label">Set Type</label>
            <select class="form-select" v-model="selectedType">
                <option value="Exercise">Exercise</option>
                <option value="Equipment">Equipment</option>
                <option value="Behavioural">Behavioural</option>
                <option value="LifeStyle">LifeStyle</option>
            </select>
        </div>

        <div class="mb-3">
            <label class="form-label">Description</label>
            <textarea class="form-control" v-model="Description" rows="3"></textarea>
        </div>

        <div class="mb-3">
            <label class="form-label">Notes (Optional)</label>
            <textarea class="form-control" v-model="Notes" rows="2"></textarea>
        </div>

        <div v-if="hasErrors" class="alert alert-danger">
            Validation failed for {{ validationErrors.length }} record(s). They are highlighted in red. Please remove
            them or select a different status.
        </div>

        <div class="d-flex justify-content-end mt-4">
            <button type="button" class="btn btn-success" :disabled="!isSubmitEnabled" @click="handleSubmit">
                Create Suggestions for {{ validRecords.length }} Employee(s)
            </button>
        </div>
    </div>
</template>
