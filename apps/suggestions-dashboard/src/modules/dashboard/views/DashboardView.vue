<script setup lang="ts">
import SummaryCard from '../components/SummaryCard.vue';
import SuggestionsStatusChart from '../components/SuggestionsStatusChart.vue';
import SuggestionsView from '@/modules/suggestions/views/SuggestionsView.vue';
</script>

<template>
    <div class="row g-4 mb-4">
        <div class="col-md-6 col-lg-3">
            <SummaryCard title="Total Employees" value="142" icon="bi bi-people" icon-bg-class="bg-primary" />
        </div>
        <div class="col-md-6 col-lg-3">
            <SummaryCard title="Total Departments" value="12" icon="bi bi-building" icon-bg-class="bg-info" />
        </div>
        <div class="col-md-6 col-lg-3">
            <SummaryCard title="Pending Suggestions" value="8" icon="bi bi-lightbulb" icon-bg-class="bg-warning" />
        </div>
        <div class="col-md-6 col-lg-3">
            <SummaryCard title="Completed" value="76" icon="bi bi-check-circle" icon-bg-class="bg-success" />
        </div>
    </div>

    <div class="row g-4">
        <div class="col-lg-7">
            <div class="card">
                <div class="card-header">
                    <h5 class="mb-0">Suggestion Status</h5>
                </div>
                <div class="card-body">
                    <div class="row">
                        <div class="col-md-6 mb-3">
                            <div class="p-3 border rounded">
                                <h5>Pending</h5><span class="fs-2 fw-bold">8</span>
                            </div>
                        </div>
                        <div class="col-md-6 mb-3">
                            <div class="p-3 border rounded">
                                <h5>In Progress</h5><span class="fs-2 fw-bold">12</span>
                            </div>
                        </div>
                        <div class="col-md-6 mb-3 mb-md-0">
                            <div class="p-3 border rounded">
                                <h5>Completed</h5><span class="fs-2 fw-bold">76</span>
                            </div>
                        </div>
                        <div class="col-md-6">
                            <div class="p-3 border rounded">
                                <h5>Dismissed</h5><span class="fs-2 fw-bold">3</span>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div class="col-lg-5">
            <div class="card h-100">
                <div class="card-header">
                    <h5 class="mb-0">Suggestions Breakdown by Status</h5>
                </div>
                <div class="card-body d-flex justify-content-center align-items-center">
                    <SuggestionsStatusChart />
                    <!-- <SuggestionsStatusChart :chartData="chartData" :chartOptions="chartOptions" /> -->
                </div>
            </div>
        </div>
        <div class="col-lg-12">
            <SuggestionsView />
        </div>
    </div>
</template>