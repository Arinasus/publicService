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
      <link href="https://fonts.googleapis.com/css2?family=Material+Symbols+Outlined" rel="stylesheet" />
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
          <td>
            <span class="role" :class="u.role">
              <span class="material-symbols-outlined">
                {{ u.role === 'admin' ? 'shield_person' : 'person' }}
              </span>
              {{ u.role === 'admin' ? 'Администратор' : 'Пользователь' }}
            </span>
          </td>
          <td class="actions">
            <button class="btn-delete" @click="deleteUser(u)">
              <span class="material-symbols-outlined">delete</span> Удалить
            </button>
            <button class="btn-admin" @click="changeRole(u, 'admin')">
              <span class="material-symbols-outlined">shield_person</span> Сделать администатором
            </button>
            <button class="btn-user" @click="changeRole(u, 'user')">
              <span class="material-symbols-outlined">person</span> Сделать пользователем
            </button>
          </td>
        </tr>
      </tbody>
    </table>
  </div>
</template>


<style scoped>
.page {
  padding: 30px;
  max-width: 1000px;
  margin: 0 auto;
}

h2 {
  font-size: 2rem;
  font-weight: 700;
  color: var(--color-navy);
  margin-bottom: 25px;
}

.error {
  background: #fbe7e9;
  color: var(--color-accent);
  padding: 15px;
  border-radius: var(--radius-md);
  border-left: 4px solid var(--color-accent);
  margin-bottom: 20px;
}

/* Таблица */
.users {
  width: 100%;
  border-collapse: collapse;
  box-shadow: var(--shadow-sm);
  border-radius: var(--radius-md);
  overflow: hidden;
}

.users th {
  background: var(--color-navy);
  color: white;
  padding: 12px;
  text-align: center;
}

.users td {
  padding: 12px;
  border-bottom: 1px solid #eee;
  text-align: center;
}

.users tr:hover {
  background: #f9f9f9;
}

/* Роль */
.role {
  display: inline-flex;
  align-items: center;
  gap: 6px;
  font-weight: 600;
}

.role.admin {
  color: var(--color-navy);
}

.role.user {
  color: var(--color-text);
}

.role .material-symbols-outlined {
  font-size: 20px;
}

/* Кнопки */
.actions {
  display: flex;
  gap: 8px;
  justify-content: center;
}

button {
  display: inline-flex;
  align-items: center;
  gap: 6px;
  padding: 8px 14px;
  border-radius: var(--radius-md);
  font-weight: 600;
  cursor: pointer;
  transition: var(--transition);
  border: none;
}

button .material-symbols-outlined {
  font-size: 18px;
}

/* Удалить */
.btn-delete {
  background: #f44336;
  color: white;
}

.btn-delete:hover {
  background: #d32f2f;
  transform: translateY(-2px);
}

/* Админ */
.btn-admin {
  background: #1976d2;
  color: white;
}

.btn-admin:hover {
  background: #0d47a1;
  transform: translateY(-2px);
}

/* Пользователь */
.btn-user {
  background: #4caf50;
  color: white;
}

.btn-user:hover {
  background: #2e7d32;
  transform: translateY(-2px);
}

</style>
