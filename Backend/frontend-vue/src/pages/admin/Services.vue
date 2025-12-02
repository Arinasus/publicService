<script setup lang="ts">
import { ref, onMounted } from 'vue'

type Service = {
  serviceID: number
  serviceName: string
  unitOfMeasure: string
}

const services = ref<Service[]>([])
const loading = ref(true)
const error = ref<string | null>(null)

async function loadServices() {
  try {
    const res = await fetch(import.meta.env.VITE_API_URL + '/Services')
    if (!res.ok) throw new Error(`Ошибка загрузки: ${res.status}`)
    services.value = await res.json()
  } catch (err: any) {
    error.value = err.message
  } finally {
    loading.value = false
  }
}

async function deleteService(id: number) {
  await fetch(import.meta.env.VITE_API_URL + `/Services/${id}`, { method: 'DELETE' })
  services.value = services.value.filter(s => s.serviceID !== id)
}

onMounted(loadServices)
</script>

<template>
  <div class="page">
    <h2>Управление услугами</h2>
    <div v-if="error" class="error">{{ error }}</div>
    <div v-if="loading">Загрузка...</div>

    <table v-else>
      <thead>
        <tr>
          <th>ID</th><th>Название</th><th>Описание</th><th>Цена</th><th>Действия</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="s in services" :key="s.serviceID">
          <td>{{ s.serviceID }}</td>
          <td>{{ s.serviceName }}</td>
          <td>{{ s.unitOfMeasure }}</td>
          <td>
            <button @click="deleteService(s.serviceID)">Удалить</button>
          </td>
        </tr>
      </tbody>
    </table>
  </div>
</template>
