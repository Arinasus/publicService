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
    const res = await apiFetch('/Notifications/me') 
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
    case 'Info': 
      return 'info'
    case 'Warning': 
      return 'warning'
    case 'Payment': 
      return 'payments'
    case 'Reminder': 
      return 'alarm'
    default: 
      return 'notifications'
  }
}

</script>

<template>
    <link href="https://fonts.googleapis.com/css2?family=Material+Symbols+Outlined" rel="stylesheet" />
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
  <span class="icon material-symbols-outlined">{{ getIcon(n.notificationType) }}</span>
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
.page {
  padding: 30px;
  max-width: 900px;
  margin: 0 auto;
}

/* Заголовок */
h2 {
  font-size: 2rem;
  font-weight: 700;
  color: var(--color-navy);
  margin-bottom: 25px;
}

/* Панель действий */
.actions {
  display: flex;
  flex-wrap: wrap;
  gap: 12px;
  margin-bottom: 25px;
}

.actions button {
  padding: 10px 16px;
  border-radius: var(--radius-md);
  border: 2px solid var(--color-navy);
  background: white;
  color: var(--color-navy);
  font-weight: 600;
  cursor: pointer;
  transition: var(--transition);
}

.actions button:hover {
  background: var(--color-navy);
  color: white;
  transform: translateY(-2px);
}

/* Ошибки */
.error {
  background: #fbe7e9;
  color: var(--color-accent);
  padding: 15px;
  border-radius: var(--radius-md);
  border-left: 4px solid var(--color-accent);
  margin-bottom: 20px;
}

/* Список уведомлений */
.notifications {
  list-style: none;
  padding: 0;
  margin: 0;
}

.notifications li {
  background: white;
  padding: 18px 20px;
  border-radius: var(--radius-md);
  box-shadow: var(--shadow-sm);
  margin-bottom: 15px;
  display: flex;
  align-items: flex-start;
  gap: 15px;
  transition: var(--transition);
}

.notifications li:hover {
  box-shadow: var(--shadow-md);
  transform: translateY(-2px);
}

/* Прочитанные / непрочитанные */
.notifications li.unread {
  border-left: 4px solid var(--color-accent);
  background: #fff6f7;
}
.icon.material-symbols-outlined {
  font-size: 28px;
  color: var(--color-navy);
  margin-top: 2px;
}

.notifications li.read {
  opacity: 0.7;
}

/* Иконка */
.icon {
  font-size: 1.6rem;
  margin-top: 2px;
}

/* Дата */
.date {
  font-size: 0.85rem;
  color: var(--color-muted);
  min-width: 140px;
}

/* Тип */
.type {
  font-weight: 600;
  color: var(--color-navy);
  min-width: 110px;
}

/* Текст уведомления */
.message {
  flex: 1;
  font-size: 0.95rem;
  color: var(--color-text);
}

/* Кнопка "прочитано" */
.notifications button {
  padding: 8px 14px;
  border-radius: var(--radius-md);
  border: 2px solid var(--color-accent);
  background: white;
  color: var(--color-accent);
  font-weight: 600;
  cursor: pointer;
  transition: var(--transition);
  white-space: nowrap;
}

.notifications button:hover {
  background: var(--color-accent);
  color: white;
  transform: translateY(-2px);
}

/* Архив */
h3 {
  margin-top: 40px;
  font-size: 1.4rem;
  font-weight: 600;
  color: var(--color-navy);
}

ul.archive {
  list-style: none;
  padding: 0;
}

.archive li {
  padding: 10px 0;
  border-bottom: 1px solid #eee;
  color: var(--color-muted);
}

</style>
