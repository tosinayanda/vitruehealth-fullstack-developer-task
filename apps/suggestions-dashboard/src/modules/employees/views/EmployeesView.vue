<script setup lang="ts">
import { ref, reactive, onMounted, watch } from 'vue';
import FilterBar from '@/components/base/FilterBar.vue';
import BaseTable from '@/components/base/BaseTable.vue';
import router from '@/router';
import { SuggestionService } from '@/modules/suggestions/services/SuggestionService';
import { Source, SuggestionStatus, type Employee, type Suggestion } from '@/types/Suggestions';
import { useToast } from 'vue-toastification';
import { format, formatDistance, formatRelative, subDays } from 'date-fns'
import UpdateSuggestionStatusModal from '@/modules/suggestions/components/UpdateSuggestionStatusModal.vue';
import CreateSuggestionModal from '@/modules/suggestions/components/CreateSuggestionModal.vue';
import { useAuthStore } from '@/stores/auth';
import { EmployeeService } from '../services/EmployeeService';

const authStore = useAuthStore();

const suggestionService = new SuggestionService();
const employeeService = new EmployeeService();
const toast = useToast();

const selectedRecords = ref([]);
const isModalOpen = ref(false);

let modalMode = ref<'edit' | 'add'>('edit'); // 'edit' or 'add'

const recordsToUpdate = ref<any[]>([]); // Holds the records for the modal
const recordsToCreate = ref<any[]>([]); // Holds the record for creation

// --- Handler for Edit Button Click ---
const openEditModal = (record: any) => {
    modalMode.value = 'edit';
    recordsToUpdate.value = [];

    if (record) recordsToUpdate.value.push(record);

    if (selectedRecords.value.length > 0) {
        recordsToUpdate.value = [...recordsToUpdate.value, ...new Set(selectedRecords.value)]; // Remove duplicates
    }

    isModalOpen.value = true;
};

// --- Handler for Add Button Click ---
const openAddModal = (record: any) => {
    modalMode.value = 'add';
    recordsToCreate.value = [];

    if (record) recordsToCreate.value.push(record);

    isModalOpen.value = true;
};

// --- Handler for Modal Close/Submit ---

const handleCreateSuccess = (createdRecords: any[]) => {
    isModalOpen.value = false;
    loadData(filtersState);
    console.log('Records successfully created:', createdRecords);
};


var employees = ref(<Employee[]>([]));

const columns = [
    { key: 'name', label: 'Employee Name' },
    { key: 'department', label: 'Employee Department' },
    { key: 'actions', label: '' },
];

const viewEmployee = (employee: any) => {
    router.push({ name: 'employee-detail', params: { id: employee.id } });
}

// --- State Management ---
const isLoading = ref(false);
const totalPages = ref(1); // For future use
const totalRecords = ref(0); // For future use
const paging = ref({ pageNumber: 1, pageSize: 25 }); // For future use

// // --- Watcher: Re-fetch Data on Filter Change ---
// watch(filters, (newFilters, oldFilters) => {
//     console.log('Filters changed. Re-fetching data...');
//     loadData();
// }, { deep: true });

const loadData = async (filters: any) => {
    isLoading.value = true;
    employees.value = []; // Clear old data

    try {
        const response = await employeeService.getAllEmployees(paging.value.pageNumber, paging.value.pageSize);
        // toast.success('Suggestions fetched successfully.');
        console.log('Fetched employees:', response);
        employees.value = response.data ?? [];
        totalPages.value = response.totalPages ?? 1;
        totalRecords.value = response.totalRecords ?? response.data?.length ?? 0;

    } catch (error) {
        console.error('Error fetching data:', error);
        toast.error('Failed to fetch employees.');
    } finally {
        isLoading.value = false;
    }
};

const exportDataAsCsv = () => {
    if (selectedRecords.value.length === 0) {
        console.warn('No data to export.');
        return;
    }

    // 1. Get Headers (using the keys of the first object)
    const headers = Object.keys(selectedRecords.value[0]);

    // Convert array of objects to array of arrays (rows)
    const rows = selectedRecords.value.map(obj =>
        headers.map(header => {
            // Ensure data is properly formatted (e.g., escape quotes)
            let value = obj[header] || '';
            // For simple data, we just convert to string and wrap in quotes
            return `"${String(value).replace(/"/g, '""')}"`;
        })
    );

    // 2. Combine headers and rows
    // Join array elements into CSV string
    const headerRow = headers.map(h => `"${h}"`).join(',');
    const csvContent = [headerRow, ...rows.map(row => row.join(','))].join('\n');

    // 3. Create a Blob and trigger download
    const blob = new Blob([csvContent], { type: 'text/csv;charset=utf-8;' });
    const url = URL.createObjectURL(blob);

    // Create a link element, click it, and remove it
    const link = document.createElement('a');
    link.setAttribute('href', url);

    // Use current date and time in the filename
    const date = new Date().toISOString().slice(0, 10);
    link.setAttribute('download', `data-export-${date}.csv`);

    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);

    // Clean up the URL object
    URL.revokeObjectURL(url);
};

const truncateText = (text: string, maxLength: number = 10) => {
    if (!text) return '';
    const str = String(text);
    return str.length > maxLength ? str.substring(0, maxLength) + '...' : str;
};

const filterConfig = [
    {
        key: 'department',
        label: 'Department',
        options: [
        ]
    }
];

const filtersState = reactive({
    search: '',
    department: undefined as string | undefined,
});

watch(filtersState, (newFilters) => {
    console.log('Filters changed, triggering API call:', newFilters);
    loadData(newFilters);
}, { deep: true });


const updateFilter = (key: string, event: Event) => {
    const value = (event.target as HTMLSelectElement || event.target as HTMLInputElement).value; // Use HTMLSelectElement
    const newFilterState = {
        ...filtersState,
        [key]: value
    };
    if (key === 'search') {
        // If search is updated, we might want to debounce the API call
        filtersState.search = value;
    }
    if (key === 'department') {
        filtersState.department = value;
    }
    console.log('updated filter state:', newFilterState);
};

const resetFilters = () => {
    filtersState.search = '';
    filtersState.department = '';
    console.log('reset filter state:', filtersState);
};

// --- Lifecycle Hook: Initial Data Load ---
onMounted(() => {
    loadData(filtersState);
});

const getFilterValue = (key: string) => {
    return filtersState[key as keyof typeof filtersState];
};

</script>

<template>
    <div class="card">
        <div class="card-body">

            <!-- <FilterBar :filters="filterConfig" v-model="filtersState" /> -->

            <div class="row g-2 align-items-center mb-3">

                <div class="col-md-3">
                    <input type="search" class="form-control" placeholder="Search..." :value="filtersState.search"
                        @input="updateFilter('search', $event)" />
                </div>

                <div class="col-md-3">
                    <label :for="'department'">Department</label>   
                    <select class="form-select" :value="getFilterValue('department')"
                        @change="updateFilter('department', $event)">
                    <option value="HR">HR</option>
                    <option value="Operations">Operations</option>
                    <option value="Marketing">Marketing</option>
                    <option value="Finance">Finance</option>
                    <option value="Engineering">Engineering</option>
                    </select>
                </div>

                <div class="col-auto">
                    <a href="#" @click.prevent="resetFilters">Reset</a>
                </div>
            </div>

            <div class="d-flex justify-content-end mb-3">
                <button class="btn btn-outline-secondary me-2" v-if="selectedRecords.length > 0"
                    @click="exportDataAsCsv"><i class="bi bi-upload me-1"></i> Export</button>
                <button class="btn btn-primary" @click="openAddModal" v-if="selectedRecords.length > 0"><i
                        class="bi bi-plus-lg me-1"></i> Add
                    Suggestion</button>
            </div>

            <BaseTable :columns="columns" :items="employees" :is-selectable="true"
                v-model:selected-items="selectedRecords">e="getFilterValue(filter.key)"
                        @chang
                <template #cell(name)="{ item }">
                    <a href="#" @click.prevent="viewEmployee(item)" class="text-decoration-none">{{ item.name }}</a>
                </template>
                <template #cell(department)="{ item }">
                    <span class="badge bg-success-subtle text-success-emphasis rounded-pill">{{ item.department
                    }}</span>
                </template>

                <template #cell(actions)="{ item }">
                    <div class="dropdown">
                        <button class="btn btn-sm btn-ghost" type="button" data-bs-toggle="dropdown">
                            <i class="bi bi-three-dots-vertical"></i>
                        </button>
                        <ul class="dropdown-menu">
                            <li><button class="dropdown-item" @click="openAddModal(item)"><i
                                        class="bi bi-info-circle"></i> Add Suggestion</button></li>
                        </ul>
                    </div>
                </template>
            </BaseTable>

            <nav class="d-flex justify-content-end align-items-center mt-3">
                <span>1 - {{ totalRecords < paging.pageSize ? totalRecords : paging.pageSize * paging.pageNumber }} of
                        {{ totalRecords }}</span>
                        <ul class="pagination mb-0 ms-3">
                            <li class="page-item" v-if="paging.pageNumber > 1"><a class="page-link" href="#">&laquo;</a>
                            </li>
                            <li class="page-item active"><a class="page-link" href="#">1</a></li>
                            <li class="page-item" v-if="paging.pageNumber < totalPages"><a class="page-link"
                                    href="#">&raquo;</a>
                            </li>
                        </ul>
            </nav>

            <CreateSuggestionModal v-if="isModalOpen && modalMode === 'add'" :records="recordsToCreate"
                @close="isModalOpen = false" @success="handleCreateSuccess" />
        </div>
    </div>
</template>

<style scoped>
/* For the transparent action button */
.btn-ghost {
    background: transparent;
    border: none;
}
</style>