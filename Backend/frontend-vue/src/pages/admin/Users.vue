<script setup lang="ts">
import { ref, onMounted } from 'vue'
type User = { userID: number; firstName: string; lastName: string; email: string }
const users = ref<User[]>([])

onMounted(async () => {
  const res = await fetch(import.meta.env.VITE_API_URL + '/Users', { credentials: 'omit' })
  users.value = await res.json()
})
</script>

<template>
  <div>
    <h2>Админ: Пользователи</h2>
    <ul>
      <li v-for="u in users" :key="u.userID">
        {{ u.firstName }} {{ u.lastName }} — {{ u.email }}
      </li>
    </ul>
  </div>
</template>
