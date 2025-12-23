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
  <div class="page container py-4">
    <h2 class="text-green-dark mb-4">Управление услугами</h2>

    <div v-if="error" class="alert alert-danger">{{ error }}</div>
    <div v-else-if="loading" class="text-muted">Загрузка...</div>
    <div v-else>
      <!-- форма добавления -->
      <form @submit.prevent="addService" class="row g-2 mb-4">
        <div class="col-md-3">
          <input v-model="newService.serviceName" class="form-control" placeholder="Название" required />
        </div>
        <div class="col-md-3">
          <input v-model="newService.unitOfMeasure" class="form-control" placeholder="Единица измерения" required />
        </div>
        <div class="col-md-3">
          <input v-model.number="newService.price" type="number" class="form-control" placeholder="Цена" required />
        </div>
        <div class="col-md-3">
          <button type="submit" class="btn btn-success w-100">Добавить услугу</button>
        </div>
      </form>

      <!-- таблица -->
      <table class="contracts table table-bordered table-hover align-middle">
        <thead class="table-header">
          <tr>
            <th>ID</th>
            <th>Название</th>
            <th>Ед. изм.</th>
            <th>Цена</th>
            <th>Действия</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="s in services" :key="s.serviceID">
            <td>{{ s.serviceID }}</td>
            <td>
              <span v-if="!s.editing">{{ s.serviceName }}</span>
              <input v-else v-model="s.serviceName" class="form-control" />
            </td>
            <td>
              <span v-if="!s.editing">{{ s.unitOfMeasure }}</span>
              <input v-else v-model="s.unitOfMeasure" class="form-control" />
            </td>
            <td>
              <span v-if="!s.editing">{{ s.price }} ₽</span>
              <input v-else type="number" v-model.number="s.price" class="form-control" />
            </td>
            <td>
              <div class="d-flex gap-2">
                <button v-if="!s.editing" @click="s.editing = true" class="btn btn-warning btn-sm">Редактировать</button>
                <button v-else @click="updateService(s)" class="btn btn-success btn-sm">Сохранить</button>
                <button @click="deleteService(s.serviceID)" class="btn btn-danger btn-sm">Удалить</button>
              </div>
            </td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>

<style scoped>
:root {
  --color-lemon: #FFFACD;
  --color-green-light: #A3D9A5;
  --color-green-deep: #4B8F6B;
  --color-green-dark: #2F5D3A;
  --color-yellow-green: #C4D96F;
}

.page {
  background-color: var(--color-lemon);
  min-height: 100vh;
  font-family: 'Inter', 'Roboto', sans-serif;
}

.text-green-dark {
  color: var(--color-green-dark);
}

.contracts {
  border: 2px solid var(--color-green-deep);
  border-radius: 8px;
  overflow: hidden;
  background-color: var(--color-lemon);
  box-shadow: 0 2px 6px rgba(0, 0, 0, 0.1);
}

.table-header {
  background-color: var(--color-green-light);
  color: var(--color-green-dark);
  text-transform: uppercase;
  font-weight: 600;
}

.contracts th,
.contracts td {
  text-align: center;
  padding: 12px;
  border: 1px solid var(--color-green-deep);
}

.contracts tbody tr:nth-child(even) {
  background-color: #fff;
}

.contracts tbody tr:nth-child(odd) {
  background-color: var(--color-lemon);
}

.contracts tbody tr:hover {
  background-color: var(--color-yellow-green);
  color: var(--color-green-dark);
  transition: 0.3s ease;
}
</style>
