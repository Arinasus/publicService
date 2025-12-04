<script setup lang="ts">
import { ref, onMounted } from 'vue'

type Service = {
  serviceID: number
  serviceName: string
  unitOfMeasure: string
  price: number
  editing?: boolean
}

const services = ref<Service[]>([])
const loading = ref(true)
const error = ref<string | null>(null)
// форма для новой услуги
const newService = ref<Service>({
  serviceID: 0,
  serviceName: '',
  unitOfMeasure: '',
  price: 0
})
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
async function addService() {
  try {
    const res = await fetch(import.meta.env.VITE_API_URL + '/Services', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({
        serviceName: newService.value.serviceName,
        unitOfMeasure: newService.value.unitOfMeasure,
        price: newService.value.price
      })
    })
    if (!res.ok) throw new Error(`Ошибка добавления: ${res.status}`)
    const created = await res.json()
    services.value.push(created)
    newService.value = { serviceID: 0, serviceName: '', unitOfMeasure: '', price: 0 }
  } catch (err: any) {
    error.value = err.message
  }
}
async function updateService(service: Service) {
  await fetch(import.meta.env.VITE_API_URL + `/Services/${service.serviceID}`, {
    method: 'PUT',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(service)
  })
  service.editing = false
}

async function deleteService(id: number) {
  if (!confirm('Вы уверены, что хотите удалить услугу?')) return
  await fetch(import.meta.env.VITE_API_URL + `/Services/${id}`, { method: 'DELETE' })
  services.value = services.value.filter(s => s.serviceID !== id)
}

onMounted(loadServices)
</script>

<template>
  <div class="page">
    <h2>Управление услугами</h2>

    <div v-if="error" class="error">{{ error }}</div>
    <div v-else-if="loading">Загрузка...</div>
    <div v-else>
      <!-- форма добавления -->
      <form @submit.prevent="addService" class="add-form">
        <input v-model="newService.serviceName" placeholder="Название" required />
        <input v-model="newService.unitOfMeasure" placeholder="Единица измерения" required />
        <input v-model.number="newService.price" type="number" placeholder="Цена" required />
        <button type="submit">Добавить услугу</button>
      </form>

      <!-- таблица -->
      <table>
        <thead>
          <tr>
            <th>ID</th><th>Название</th><th>Ед. изм.</th><th>Цена</th><th>Действия</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="s in services" :key="s.serviceID">
            <td>{{ s.serviceID }}</td>
            <td>
              <span v-if="!s.editing">{{ s.serviceName }}</span>
              <input v-else v-model="s.serviceName" />
            </td>
            <td>
              <span v-if="!s.editing">{{ s.unitOfMeasure }}</span>
              <input v-else v-model="s.unitOfMeasure" />
            </td>
            <td>
              <span v-if="!s.editing">{{ s.price }} ₽</span>
              <input v-else type="number" v-model.number="s.price" />
            </td>
            <td>
              <button v-if="!s.editing" @click="s.editing = true">Редактировать</button>
              <button v-else @click="updateService(s)">Сохранить</button>
              <button @click="deleteService(s.serviceID)">Удалить</button>
            </td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>

<style scoped>
.page { padding: 20px; }
.error { color: red; margin-bottom: 10px; }
.add-form { margin-bottom: 20px; display: flex; gap: 10px; }
.add-form input { padding: 5px; }
table { width: 100%; border-collapse: collapse; }
th, td { border: 1px solid #ddd; padding: 8px; }
th { background: #f5f5f5; }
input { width: 100%; border: none; background: #fafafa; padding: 4px; }
button { padding: 5px 10px; cursor: pointer; }
</style>