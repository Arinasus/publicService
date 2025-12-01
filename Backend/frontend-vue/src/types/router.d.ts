import 'vue-router'

type Role = 'admin' | 'user'

declare module 'vue-router' {
  interface RouteMeta {
    auth?: boolean
    role?: Role[]
  }
}
