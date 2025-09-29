<script setup lang="ts">
import { Doughnut } from 'vue-chartjs'
import { Chart as ChartJS, Title, Tooltip, Legend, ArcElement } from 'chart.js'
ChartJS.register(Title, Tooltip, Legend, ArcElement)

const chartData = {
  labels: ['Pending', 'In Progress', 'Completed' , 'Dismissed'],
  datasets: [
    {
      backgroundColor: ['#198754', '#dc3545', '#ffc107', '#6c757d'],
      data: [111, 10, 30, 40] // Example data
    }
  ]
};

const chartOptions = {
  responsive: true,
  maintainAspectRatio: false,
  cutout: '75%', // Makes it a donut chart
  plugins: {
    legend: {
      position: 'bottom' as const,
    }
  }
};
</script>
<template>
  <Doughnut :data="chartData" :options="chartOptions" />
</template>