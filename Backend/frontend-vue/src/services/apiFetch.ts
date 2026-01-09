// src/services/apiFetch.ts
export async function apiFetch(path: string, options: RequestInit = {}) {
  const raw = localStorage.getItem('auth')
  const headers = new Headers(options.headers || {})

  if (raw) {
    const auth = JSON.parse(raw)
    if (auth?.token) {
      headers.set('Authorization', `Bearer ${auth.token}`)
    }
  }

  return fetch(import.meta.env.VITE_API_URL + path, {
    ...options,
    headers,
  })
}
