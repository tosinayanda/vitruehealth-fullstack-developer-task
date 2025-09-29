<template>
  <div class="offcanvas offcanvas-start bg-dark text-white" tabindex="-1" id="sidebarOffcanvas" aria-labelledby="sidebarOffcanvasLabel">
    <div class="offcanvas-header">
      <h5 class="offcanvas-title" id="sidebarOffcanvasLabel">Menu</h5>
      <button type="button" class="btn-close btn-close-white" data-bs-dismiss="offcanvas" aria-label="Close"></button>
    </div>
    <div class="offcanvas-body">
      <ul class="nav flex-column">
         <li class="nav-item">
          <RouterLink to="/dashboard" class="nav-link text-white"><i class="bi bi-grid me-2"></i>Dashboard</RouterLink>
        </li>
        <li class="nav-item">
          <RouterLink to="/employees" class="nav-link text-white"><i class="bi bi-people me-2"></i>Employees</RouterLink>
        </li>
        <li class="nav-item">
          <RouterLink to="/suggestions" class="nav-link text-white"><i class="bi bi-lightbulb me-2"></i>Suggestions</RouterLink>
        </li>
      </ul>
    </div>
  </div>

  <div class="d-none d-md-flex flex-column flex-shrink-0 p-3 text-white bg-dark" style="width: 280px; min-height: 100vh;">
    <a href="/" class="d-flex align-items-center mb-3 mb-md-0 me-md-auto text-white text-decoration-none">
      <span class="fs-4">Dashboard App</span>
    </a>
    <hr>
    <ul class="nav nav-pills flex-column mb-auto">
      <li class="nav-item">
        <RouterLink to="/dashboard" class="nav-link text-white" active-class="active"><i class="bi bi-grid me-2"></i>Dashboard</RouterLink>
      </li>
      <li>
        <RouterLink to="/employees" class="nav-link text-white" active-class="active"><i class="bi bi-people me-2"></i>Employees</RouterLink>
      </li>
      <li>
        <RouterLink to="/suggestions" class="nav-link text-white" active-class="active"><i class="bi bi-lightbulb me-2"></i>Suggestions</RouterLink>
      </li>
    </ul>
  </div>
</template>