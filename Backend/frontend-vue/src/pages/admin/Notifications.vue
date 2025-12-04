<script setup lang="ts">
import { ref, onMounted } from 'vue'

// DTO для отображения уведомлений (из API)
type Notification = {
  notificationID: number
  notificationDate: string
  notificationType: string
  notificationText: string
  isRead: boolean
  userEmail: string
  lastUpdatedDate?: string
}

// DTO для пользователей (из API)
type User = {
  userID: number
  firstName: string
  lastName: string
  email: string
}

// Форма для добавления/редактирования уведомления
type NotificationForm = {
  notificationID?: number
  notificationDate: string
  notificationType: string
  notificationText: string
  isRead: boolean
  userID: number
}

const notifications = ref<Notification[]>([])
const users = ref<User[]>([])
const loading = ref(true)
const error = ref<string | null>(null)

const form = ref<NotificationForm>({
  notificationDate: new Date().toISOString(),
  notificationType: '',
  notificationText: '',
  isRead: false,
  userID: 0
})
const editingId = ref<number | null>(null)

async function loadNotifications() {
  try {
    const res = await fetch(import.meta.env.VITE_API_URL + '/Notifications')
    if (!res.ok) throw new Error(`Ошибка загрузки: ${res.status}`)
    notifications.value = await res.json()
  } catch (err: any) {
    error.value = err.message
  } finally {
    loading.value = false
  }
}

async function loadUsers() {
  try {
    const res = await fetch(import.meta.env.VITE_API_URL + '/Users')
    if (!res.ok) throw new Error(`Ошибка загрузки пользователей: ${res.status}`)
    users.value = await res.json()
  } catch (err: any) {
    error.value = err.message
  }
}
const sendToAll = ref(false) // один раз объявляем

async function saveNotification() {
  // если выбрано "отправить всем"
  if (sendToAll.value) {
    for (const u of users.value) {
      const body = {
        notificationDate: form.value.notificationDate,
        notificationType: form.value.notificationType,
        notificationText: form.value.notificationText,
        isRead: false,
        userID: u.userID
      }
      await fetch(import.meta.env.VITE_API_URL + '/Notifications', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(body)
      })
    }
    await loadNotifications()
    resetForm()
    return
  }

  // обычное сохранение/редактирование
  const url = import.meta.env.VITE_API_URL + '/Notifications' + (editingId.value ? `/${editingId.value}` : '')
  const method = editingId.value ? 'PUT' : 'POST'

  const body = {
    notificationID: editingId.value ?? 0,
    notificationDate: form.value.notificationDate,
    notificationType: form.value.notificationType,
    notificationText: form.value.notificationText,
    isRead: form.value.isRead,
    userID: form.value.userID
  }

  const res = await fetch(url, {
    method,
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(body)
  })

  if (res.ok) {
    await loadNotifications()
    resetForm()
  } else {
    error.value = `Ошибка сохранения: ${res.status}`
  }
}

function editNotification(n: Notification) {
  editingId.value = n.notificationID
  const user = users.value.find(u => u.email === n.userEmail)
  form.value = {
    notificationID: n.notificationID,
    notificationDate: n.notificationDate,
    notificationType: n.notificationType,
    notificationText: n.notificationText,
    isRead: n.isRead,
    userID: user ? user.userID : 0
  }
}


async function deleteNotification(id: number) {
  const res = await fetch(import.meta.env.VITE_API_URL + `/Notifications/${id}`, { method: 'DELETE' })
  if (res.ok) {
    notifications.value = notifications.value.filter(n => n.notificationID !== id)
  } else {
    error.value = `Ошибка удаления: ${res.status}`
  }
}

function resetForm() {
  editingId.value = null
  form.value = {
    notificationDate: new Date().toISOString(),
    notificationType: '',
    notificationText: '',
    isRead: false,
    userID: 0
  }
}

onMounted(() => {
  loadNotifications()
  loadUsers()
})
</script>

<template>
  <div class="page">
    <h2>Управление уведомлениями</h2>

    <div v-if="error" class="error">{{ error }}</div>
    <div v-if="loading">Загрузка...</div>

    <!-- форма -->
    <div class="form">
      <h3>{{ editingId ? 'Редактировать уведомление' : 'Добавить уведомление' }}</h3>
      <select v-model="form.notificationType">
        <option value="Info">Информация</option>
        <option value="Warning">Предупреждение</option>
        <option value="Payment">Оплата</option>
        <option value="Reminder">Напоминание</option>
      </select>
      <textarea v-model="form.notificationText" placeholder="Текст уведомления"></textarea>
      <label>
        <input type="checkbox" v-model="form.isRead" /> Прочитано
      </label>
       <select v-model="form.userID" :disabled="sendToAll">
        <option disabled value="0">Выберите пользователя</option>
        <option v-for="u in users" :key="u.userID" :value="u.userID">
          {{ u.firstName }} {{ u.lastName }} ({{ u.email }})
        </option>
      </select>
        <label v-if="!editingId">
            <input type="checkbox" v-model="sendToAll" /> Отправить всем пользователям
        </label>
      <button @click="saveNotification">{{ editingId ? 'Сохранить' : 'Добавить' }}</button>
      <button @click="resetForm" v-if="editingId">Отмена</button>
    </div>

    <!-- список -->
    <table>
      <thead>
        <tr>
          <th>ID</th><th>Дата</th><th>Тип</th><th>Текст</th><th>Прочитано</th><th>Email</th><th>Последнее изменение</th><th>Действия</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="n in notifications" :key="n.notificationID">
          <td>{{ n.notificationID }}</td>
          <td>{{ new Date(n.notificationDate).toLocaleString('ru-RU') }}</td>
          <td>{{ n.notificationType }}</td>
          <td>
                {{ n.notificationText }}
                <span v-if="n.lastUpdatedDate"> (Изменено {{ new Date(n.lastUpdatedDate).toLocaleString('ru-RU') }})</span>
          </td>
          <td>{{ n.isRead ? 'Да' : 'Нет' }}</td>
          <td>{{ n.userEmail }}</td>
          <td>
        <span v-if="n.lastUpdatedDate">
          {{ new Date(n.lastUpdatedDate).toLocaleString('ru-RU') }}
        </span>
        <span v-else>-</span>
      </td>
       <td>
            <button @click="editNotification(n)">✏️</button>
            <button @click="deleteNotification(n.notificationID)">❌</button>
          </td>
        </tr>
      </tbody>
    </table>
  </div>
</template>

<style scoped>
.page { padding: 20px; }
.error { color: red; margin-bottom: 10px; }
.form { margin-bottom: 20px; display: flex; flex-direction: column; gap: 10px; }
table { width: 100%; border-collapse: collapse; margin-top: 20px; }
th, td { border: 1px solid #ddd; padding: 8px; }
th { background: #f5f5f5; }
button { margin-right: 5px; }
</style>
