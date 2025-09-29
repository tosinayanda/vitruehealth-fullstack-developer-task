// src/stores/auth.ts
import { reactive, readonly } from 'vue';

// 1. Define the shape of our state
interface AuthState {
  username: string | null;
  id: number | null;
  isAuthenticated: boolean;
}

// 2. Create the reactive state object
// This is the core of our store. `reactive()` makes it trackable.
const state = reactive<AuthState>({
  username: localStorage.getItem('username') || null,
  id: localStorage.getItem('id') ? Number(localStorage.getItem('id')) : null,
  isAuthenticated: !!localStorage.getItem('username'),
});

// 3. Create actions to mutate the state
// These functions are the only way components should modify the state.
function setUsername(name: string) {
  state.username = name;
  state.isAuthenticated = true;
  localStorage.setItem('username', name);
}

function setId(id: number) {
  state.id = id;
  localStorage.setItem('id', String(id));
}

function logout() {
  state.username = null;
  state.id = null;
  state.isAuthenticated = false;
  localStorage.removeItem('username');
  localStorage.removeItem('id');
}

// 4. Export a composable function to be used in components
// We return a `readonly` version of the state to prevent direct mutation
// from outside the store, which is a good practice.
export function useAuthStore() {
  return {
    state: readonly(state),
    setUsername,
    setId,
    logout,
  };
}