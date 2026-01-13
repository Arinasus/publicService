<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { apiFetch } from '../../services/apiFetch' 
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
    const res = await apiFetch('/Notifications') 
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
    const res = await apiFetch('/Users') 
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
      await apiFetch('/Notifications', {   // ✅ заменили fetch
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
  const url = '/Notifications' + (editingId.value ? `/${editingId.value}` : '')
  const method = editingId.value ? 'PUT' : 'POST'
  const body = {
    notificationID: editingId.value ?? 0,
    notificationDate: form.value.notificationDate,
    notificationType: form.value.notificationType,
    notificationText: form.value.notificationText,
    isRead: form.value.isRead,
    userID: form.value.userID
  }

  const res = await apiFetch(url, {   // ✅ заменили fetch
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
  <link href="https://fonts.googleapis.com/css2?family=Material+Symbols+Outlined" rel="stylesheet"/>
  <div class="page">
    <h2>
      <span class="material-symbols-outlined">notifications</span>
      Управление уведомлениями
    </h2>

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
      <div class="actions">
        <button class="btn-save" @click="saveNotification">
          <span class="material-symbols-outlined">{{ editingId ? 'save' : 'add_circle' }}</span>
          {{ editingId ? 'Сохранить' : 'Добавить' }}
        </button>
        <button v-if="editingId" class="btn-cancel" @click="resetForm">
          <span class="material-symbols-outlined">cancel</span> Отмена
        </button>
      </div>
    </div>

    <!-- список -->
    <table class="contracts">
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
          <td>{{ n.lastUpdatedDate ? new Date(n.lastUpdatedDate).toLocaleString('ru-RU') : '-' }}</td>
          <td class="actions">
            <button class="btn-edit" @click="editNotification(n)">
              <span class="material-symbols-outlined">edit</span> Редактировать
            </button>
            <button class="btn-delete" @click="deleteNotification(n.notificationID)">
              <span class="material-symbols-outlined">delete</span> Удалить
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
  font-family: 'Inter', 'Roboto', sans-serif;
}

h2 {
  font-size: 2rem;
  font-weight: 700;
  color: var(--color-navy);
  margin-bottom: 25px;
  display: flex;
  align-items: center;
  gap: 10px;
}

.error {
  background: #fbe7e9;
  color: var(--color-accent);
  padding: 15px;
  border-radius: var(--radius-md);
  border-left: 4px solid var(--color-accent);
  margin-bottom: 20px;
}

/* Форма */
.form {
  background: #f8f9fa;
  padding: 20px;
  border-radius: var(--radius-md);
  box-shadow: var(--shadow-sm);
  margin-bottom: 25px;
}

.form h3 {
  margin-bottom: 15px;
  color: var(--color-navy);
}

.form select,
.form textarea,
.form input[type="text"],
.form input[type="number"] {
  width: 100%;
  padding: 10px;
  margin-bottom: 12px;
  border-radius: var(--radius-md);
  border: 1px solid #ccc;
}

.form textarea {
  min-height: 80px;
  resize: vertical;
}

.actions {
  display: flex;
  gap: 10px;
  margin-top: 10px;
}

/* Кнопки */
button {
  display: inline-flex;
  align-items: center;
  gap: 6px;
  padding: 8px 14px;
  border-radius: var(--radius-md);
  font-weight: 600;
  cursor: pointer;
  border: none;
  transition: background 0.3s ease, transform 0.2s ease;
}

button .material-symbols-outlined {
  font-size: 18px;
}

.btn-save {
  background: #4caf50;
  color: white;
}
.btn-save:hover { background: #388e3c; transform: translateY(-1px); }

.btn-cancel {
  background: #9e9e9e;
  color: white;
}
.btn-cancel:hover { background: #616161; transform: translateY(-1px); }

.btn-edit {
  background: #ff9800;
  color: white;
}
.btn-edit:hover { background: #f57c00; transform: translateY(-1px); }

.btn-delete {
  background: #f44336;
  color: white;
}
.btn-delete:hover { background: #d32f2f; transform: translateY(-1px); }

/* Таблица */
.contracts {
  width: 100%;
  border-collapse: separate !important;
  border-spacing: 0 !important;
  border: 2px solid var(--color-navy);
  border-radius: 12px;
  overflow: hidden;
  box-shadow: var(--shadow-sm);
}

.contracts th {
  background: var(--color-navy);
  color: white;
  padding: 12px;
  text-align: center;
}

.contracts td {
  padding: 12px;
  border-bottom: 1px solid #eee;
  text-align: center;
}

.contracts tr:hover {
  background: #f9f9f9;
}

</style>
