import { createRouter, createWebHistory, type RouteRecordRaw, type RouteLocationNormalized } from 'vue-router'

type Role = 'admin' | 'user'
type AuthState = { token: string; role: Role } | null

function getAuth(): AuthState {
  const raw = localStorage.getItem('auth')
  return raw ? JSON.parse(raw) : null
}

const routes: RouteRecordRaw[] = [
  { path: '/', name: 'home', component: () => import('../pages/Home.vue') },
  { path: '/login', name: 'login', component: () => import('../pages/Login.vue') },

  // user
  {
    path: '/me',
    name: 'profile',
    component: () => import('../pages/user/Profile.vue'),
    meta: { auth: true, role: ['user', 'admin'] as Role[] },
  },
  {
    path: '/my-services',
    name: 'my-services',
    component: () => import('../pages/user/MyServices.vue'),
    meta: { auth: true, role: ['user', 'admin'] as Role[] },
  },
  {
    path: '/my-invoices',
    name: 'my-invoices',
    component: () => import('../pages/user/MyInvoices.vue'),
    meta: { auth: true, role: ['user', 'admin'] as Role[] },
  },

  // admin
  {
    path: '/admin',
    name: 'admin-dashboard',
    component: () => import('../pages/admin/Dashboard.vue'),
    meta: { auth: true, role: ['admin'] as Role[] },
  },
  {
    path: '/admin/users',
    name: 'admin-users',
    component: () => import('../pages/admin/Users.vue'),
    meta: { auth: true, role: ['admin'] as Role[] },
  },
  {
    path: '/admin/services',
    name: 'admin-services',
    component: () => import('../pages/admin/Services.vue'),
    meta: { auth: true, role: ['admin'] as Role[] },
  },
  {
    path: '/admin/invoices',
    name: 'admin-invoices',
    component: () => import('../pages/admin/Invoices.vue'),
    meta: { auth: true, role: ['admin'] as Role[] },
  },

  { path: '/:pathMatch(.*)*', name: 'notfound', component: () => import('../pages/NotFound.vue') },
]

const router = createRouter({
  history: createWebHistory(),
  routes,
})

router.beforeEach((to: RouteLocationNormalized) => {
  const auth = getAuth()

  // Требуется авторизация
  if (to.meta && (to.meta as any).auth && !auth?.token) {
    return { name: 'login', query: { redirect: to.fullPath } }
  }

  // Проверка роли
  const allowedRoles = (to.meta && (to.meta as any).role) as Role[] | undefined
  if (allowedRoles && auth?.role && !allowedRoles.includes(auth.role)) {
    return { name: auth.role === 'admin' ? 'admin-dashboard' : 'home' }
  }
})

export default router
