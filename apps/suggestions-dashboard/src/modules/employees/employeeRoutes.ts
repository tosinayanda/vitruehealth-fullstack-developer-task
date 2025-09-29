const employeeRoutes = [
  {
    path: '/employees',
    name: 'employees',
    component: () => import('@/modules/employees/views/EmployeesView.vue'),
  },
  {
    path: '/employees/:id',
    name: 'employee-detail',
    component: () => import('@/modules/employees/views/EmployeeDetailView.vue'),
    props: true,
  },
]
export default employeeRoutes
