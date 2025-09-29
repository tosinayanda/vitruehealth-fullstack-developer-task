const suggestionRoutes = [
  {
    path: '/suggestions',
    name: 'suggestions',
    component: () => import('@/modules/suggestions/views/SuggestionsView.vue'),
  },
]
export default suggestionRoutes
