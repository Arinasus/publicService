<script setup lang="ts">
import {ref, onMounted } from 'vue'
type Notification = {
    notificationID: number
    userID: number
    notificationDate: string
    notificationType: string
    notificationText: string
    isRead: boolean
}
const notifications = ref<Notification[]>([])
const loading = ref(true)
const error = ref<string | null>(null)
onMounted(async()=>{
    try{
        const res = await fetch(import.meta.env.VITE_API_URL + '/Notifications', {
        headers: { 'Content-Type': 'application/json' },
        })
        if(!res.ok) throw new Error(`Ошибка загрузки: ${res.status}`)
        notifications.value = await res.json()
    }
    catch(err: any) {
        error.value = err.message
    } finally{ loading. value = false}
})
</script>
<template>
  <div class="page">
    <h2>Мои уведомления</h2>

    <div v-if="error" class="error">{{ error }}</div>
    <div v-if="loading">Загрузка...</div>

    <ul v-else class="notifications">
      <li v-for="n in notifications" :key="n.notificationID" :class="{ read: n.isRead }">
        <span class="date">{{ new Date(n.notificationDate).toLocaleString('ru-RU') }}</span>
        <span class="type">[{{ n.notificationType }}]</span>
        <span class="message">{{ n.notificationText }}</span>
      </li>
    </ul>
  </div>
</template>

<style scoped>
.page { padding: 20px; }
.error { color: red; margin-bottom: 10px; }
.notifications { list-style: none; padding: 0; }
.notifications li { padding: 8px; border-bottom: 1px solid #ddd; }
.notifications li.read { color: #888; }
.date { margin-right: 10px; font-size: 0.9em; color: #555; }
.type { margin-right: 10px; font-weight: bold; }
</style>