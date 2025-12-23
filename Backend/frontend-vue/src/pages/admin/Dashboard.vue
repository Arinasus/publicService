<script setup lang="ts">
import { ref, onMounted } from 'vue'

const stats = ref({ users: 0, admins: 0, services: 0, unpaidInvoices: 0 })

onMounted(async () => {
  try {
    const res = await fetch(import.meta.env.VITE_API_URL + "/admin/stats")
    if (!res.ok) throw new Error(`Ошибка загрузки: ${res.status}`)
    stats.value = await res.json()
  } catch (err: any) {
    alert(`Ошибка загрузки статистики: ${err.message}`)
  }
})
</script>

<template>
  <div>
    <h2>Админ: Панель</h2>
    <ul>
      <li>Пользователей: {{ stats.users }}</li>
      <li>Админов: {{ stats.admins }}</li>
      <li>Услуг: {{ stats.services }}</li>
      <li>Неоплаченные счета: {{ stats.unpaidInvoices }}</li>
    </ul>
  </div>
</template>
