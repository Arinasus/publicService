<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { apiFetch } from '../../services/apiFetch'
type User = { 
  userID: number
  email: string
  firstName: string
  lastName: string
  role: 'admin' | 'user'
}
const users = ref<User[]>([])
const loading = ref(true)
const error = ref<string | null>(null)

onMounted(async () => {
  try {
  const res = await apiFetch('/Users')
    if (!res.ok) {
      throw new Error(`Ошибка загрузки: ${res.status}`)
    }
    users.value = await res.json()
  } catch (err: any) {
    error.value = err.message
  } finally {
    loading.value = false
  }
})

// Удаление пользователя
async function deleteUser(user: User) {
  try {
    const res = await fetch(
      import.meta.env.VITE_API_URL + `/Users/${user.userID}`,
      { method: 'DELETE' }
    )
    if (!res.ok) {
      throw new Error(`Ошибка удаления: ${res.status}`)
    }
    // Убираем пользователя из списка локально
    users.value = users.value.filter(u => u.userID !== user.userID)
  } catch (err: any) {
    alert(`Ошибка удаления: ${err.message}`)
  }
}
async function changeRole(user: User, role: 'admin'|'user') {
  try {
    const res = await fetch(
      import.meta.env.VITE_API_URL + `/Users/${user.userID}/role`,
      {
        method: 'PUT',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ role })
      }
    )
    if (!res.ok) throw new Error(`Ошибка изменения роли: ${res.status}`)
    // обновляем локально
    user.role = role
  } catch (err: any) {
    alert(`Ошибка: ${err.message}`)
  }
}

</script>

<template>
  <div class="page">
    <h2>Пользователи</h2>

    <div v-if="error" class="error">{{ error }}</div>
    <div v-if="loading">Загрузка...</div>

    <table v-else class="users">
      <thead>
        <tr>
          <th>ID</th>
          <th>Email</th>
          <th>Имя</th>
          <th>Фамилия</th>
          <th>Роль</th>
          <th>Действие</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="u in users" :key="u.userID">
          <td>{{ u.userID }}</td>
          <td>{{ u.email }}</td>
          <td>{{ u.firstName }}</td>
          <td>{{ u.lastName }}</td>
          <td>{{ u.role === 'admin' ? '⚙️ Администратор' : '👤 Пользователь' }}</td>
          <td>
  <button class="btn-delete" @click="deleteUser(u)">Удалить</button>
<button class="btn-admin" @click="changeRole(u, 'admin')">Назначить админом</button>
<button class="btn-user" @click="changeRole(u, 'user')">Сделать пользователем</button>

</td>

        </tr>
      </tbody>
    </table>
  </div>
</template>

<style scoped>
.page { padding: 20px; }
.error { color: red; margin-bottom: 10px; }
.users { width: 100%; border-collapse: collapse; }
.users th, .users td { border: 1px solid #ddd; padding: 8px; text-align: center; }
.users th { background-color: #f4f4f4; }
button { padding: 6px 12px; background-color: #d9534f; color: white; border: none; border-radius: 4px; cursor: pointer; }
button:hover { background-color: #c9302c; }
.btn-delete { background-color: #d9534f; }
.btn-admin { background-color: #5bc0de; }
.btn-user { background-color: #5cb85c; }

</style>
