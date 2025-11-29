<script setup lang="ts">
import { ref, onMounted } from 'vue'
const services = ref<{ name: string; status: string }[]>([])
onMounted(async () => {
  // TODO: GET /api/user/services
  services.value = [
    { name: 'Электроэнергия', status: 'Активна' },
    { name: 'Водоснабжение', status: 'Активна' },
  ]
})
</script>

<template>
  <div>
    <h2>Мои услуги</h2>
    <ul>
      <li v-for="s in services" :key="s.name">
        {{ s.name }} — {{ s.status }}
      </li>
    </ul>
  </div>
</template>
