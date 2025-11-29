<script setup lang="ts">
import { ref, onMounted } from 'vue'
const stats = ref({ users: 0, services: 0, unpaidInvoices: 0 })
onMounted(async () => {
  // TODO: GET /api/admin/stats
  stats.value = { users: 12, services: 6, unpaidInvoices: 3 }
})
</script>

<template>
  <div>
    <h2>Админ: Панель</h2>
    <ul>
      <li>Пользователей: {{ stats.users }}</li>
      <li>Услуг: {{ stats.services }}</li>
      <li>Неоплаченные счета: {{ stats.unpaidInvoices }}</li>
    </ul>
  </div>
</template>
