<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { apiFetch } from '../../services/apiFetch'

type Notification = {
  notificationID: number
  userID: number
  notificationDate: string
  notificationType: string
  notificationText: string
  isRead: boolean
  lastUpdatedDate?: string 
}

const notifications = ref<Notification[]>([])
const loading = ref(true)
const error = ref<string | null>(null)
const sortBy = ref<'date' | 'type' | 'status'>('date')

// загрузка
onMounted(async () => {
  try {
    const res = await apiFetch('/Notifications') 
    if (!res.ok) throw new Error(`Ошибка загрузки: ${res.status}`)
    notifications.value = await res.json()
  } catch (err: any) {
    error.value = err.message
  } finally {
    loading.value = false
  }
})

// одиночная пометка
async function markAsRead(id: number) {
  const res = await fetch(import.meta.env.VITE_API_URL + `/Notifications/${id}/read`, {
    method: 'PUT',
    headers: { 'Content-Type': 'application/json' }
  })
  if (res.ok) {
    const notification = notifications.value.find(n => n.notificationID === id)
    if (notification) notification.isRead = true
  } else {
    error.value = `Ошибка обновления: ${res.status}`
  }
}

// сортировка
const sortedNotifications = computed(() => {
  return [...notifications.value].sort((a, b) => {
    if (sortBy.value === 'date') {
      return new Date(b.notificationDate).getTime() - new Date(a.notificationDate).getTime()
    }
    if (sortBy.value === 'type') {
      return a.notificationType.localeCompare(b.notificationType)
    }
    if (sortBy.value === 'status') {
      return Number(a.isRead) - Number(b.isRead)
    }
    return 0
  })
})

// архив
const archive = computed(() => {
  const cutoff = new Date()
  cutoff.setDate(cutoff.getDate() - 30) // старше 30 дней
  return notifications.value.filter(n => n.isRead && new Date(n.notificationDate) < cutoff)
})

// массовые действия
async function markAllRead() {
  for (const n of notifications.value.filter(n => !n.isRead)) {
    await fetch(import.meta.env.VITE_API_URL + `/Notifications/${n.notificationID}/read`, { method: 'PUT' })
    n.isRead = true
  }
}

async function deleteAllRead() {
  for (const n of notifications.value.filter(n => n.isRead)) {
    await fetch(import.meta.env.VITE_API_URL + `/Notifications/${n.notificationID}`, { method: 'DELETE' })
  }
  notifications.value = notifications.value.filter(n => !n.isRead)
}

// иконки
function getIcon(type: string) {
  switch (type) {
    case 'Info': return 'ℹ️'
    case 'Warning': return '⚠️'
    case 'Payment': return '💰'
    case 'Reminder': return '⏰'
    default: return '📩'
  }
}
</script>

<template>
  <div class="page">
    <h2>Мои уведомления</h2>

    <div class="actions">
      <button @click="sortBy = 'date'">Сортировать по дате</button>
      <button @click="sortBy = 'type'">По типу</button>
      <button @click="sortBy = 'status'">По статусу</button>
      <button @click="markAllRead">Отметить все прочитанными</button>
      <button @click="deleteAllRead">Удалить все прочитанные</button>
    </div>

    <div v-if="error" class="error">{{ error }}</div>
    <div v-if="loading">Загрузка...</div>

    <ul v-else class="notifications">
      <li v-for="n in sortedNotifications" :key="n.notificationID" :class="{ read: n.isRead, unread: !n.isRead }">
        <span class="icon">{{ getIcon(n.notificationType) }}</span>
        <span class="date">{{ new Date(n.notificationDate).toLocaleString('ru-RU') }}</span>
        <span class="type">[{{ n.notificationType }}]</span>
        <span class="message">
          {{ n.notificationText }}
          <span v-if="n.lastUpdatedDate">
            (Изменено {{ new Date(n.lastUpdatedDate).toLocaleString('ru-RU') }})
          </span>
        </span>
        <button v-if="!n.isRead" @click="markAsRead(n.notificationID)">Отметить прочитанным</button>
      </li>
    </ul>

    <h3>Архив</h3>
    <ul>
      <li v-for="n in archive" :key="n.notificationID">
        {{ n.notificationText }} ({{ new Date(n.notificationDate).toLocaleDateString('ru-RU') }})
      </li>
    </ul>
  </div>
</template>

<style scoped>
.page { padding: 20px; }
.actions { margin-bottom: 10px; display: flex; gap: 10px; }
.error { color: red; margin-bottom: 10px; }
.notifications { list-style: none; padding: 0; }
.notifications li { padding: 8px; border-bottom: 1px solid #ddd; display: flex; align-items: center; gap: 10px; }
.notifications li.read { color: #888; }
.notifications li.unread { background: #ffecec; font-weight: bold; }
.date { margin-right: 10px; font-size: 0.9em; color: #555; }
.type { margin-right: 10px; font-weight: bold; }
button { margin-left: auto; }
.icon { margin-right: 8px; }
</style>
