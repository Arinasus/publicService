<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { apiFetch } from '../../services/apiFetch'

type Stats = {
  users: number
  admins: number
  services: number
  unpaidInvoices: number
}

type Provider = {
  providerID: number
  providerName: string
  contactPerson: string
  phoneNumber: string
  email: string
  iban: string
  bic: string
  unp: string
  editing?: boolean
}

const stats = ref<Stats>({ users: 0, admins: 0, services: 0, unpaidInvoices: 0 })
const providers = ref<Provider[]>([])
const newProvider = ref<Omit<Provider, 'providerID' | 'editing'>>({
  providerName: '',
  contactPerson: '',
  phoneNumber: '',
  email: '',
  iban: '',
  bic: '',
  unp: ''
})
const error = ref<string | null>(null)
const loading = ref(true)

async function loadStats() {
  try {
    const res = await apiFetch('/admin/stats')
    if (!res.ok) throw new Error(`Ошибка загрузки: ${res.status}`)
    stats.value = await res.json()
  } catch (err: any) {
    error.value = err.message
  }
}

async function loadProviders() {
  try {
    const res = await apiFetch('/Providers')
    if (!res.ok) throw new Error(`Ошибка загрузки провайдеров: ${res.status}`)
    providers.value = await res.json()
  } catch (err: any) {
    error.value = err.message
  } finally {
    loading.value = false
  }
}

async function addProvider() {
  try {
    // проверка на пустые поля
    for (const [key, value] of Object.entries(newProvider.value)) {
      if (!value.trim()) {
        error.value = `Поле "${key}" обязательно`
        return
      }
    }

    const res = await apiFetch('/Providers', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(newProvider.value)
    })
    if (!res.ok) throw new Error(`Ошибка добавления: ${res.status}`)
    const created: Provider = await res.json()
    providers.value.push(created)

    // очистка формы
    newProvider.value = {
      providerName: '',
      contactPerson: '',
      phoneNumber: '',
      email: '',
      iban: '',
      bic: '',
      unp: ''
    }
    error.value = null
  } catch (err: any) {
    error.value = err.message
  }
}

async function updateProvider(provider: Provider) {
  try {
    const res = await apiFetch(`/Providers/${provider.providerID}`, {
      method: 'PUT',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(provider)
    })
    if (!res.ok) throw new Error(`Ошибка обновления: ${res.status}`)
    provider.editing = false
  } catch (err: any) {
    error.value = err.message
  }
}

async function deleteProvider(id: number) {
  if (!confirm('Удалить провайдера?')) return
  try {
    const res = await apiFetch(`/Providers/${id}`, { method: 'DELETE' })
    if (!res.ok) throw new Error(`Ошибка удаления: ${res.status}`)
    providers.value = providers.value.filter(p => p.providerID !== id)
  } catch (err: any) {
    error.value = err.message
  }
}

onMounted(async () => {
  await loadStats()
  await loadProviders()
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

    <h3 class="mt-4">Управление провайдерами</h3>

    <div v-if="error" class="alert alert-danger">{{ error }}</div>
    <div v-else-if="loading">Загрузка...</div>
    <div v-else>
      <!-- форма добавления -->
      <form @submit.prevent="addProvider" class="row g-2 mb-4">
        <div class="col-md-3">
          <input v-model="newProvider.providerName" class="form-control" placeholder="Название" required />
        </div>
        <div class="col-md-3">
          <input v-model="newProvider.contactPerson" class="form-control" placeholder="Контактное лицо" required />
        </div>
        <div class="col-md-3">
          <input v-model="newProvider.phoneNumber" class="form-control" placeholder="Телефон" required />
        </div>
        <div class="col-md-3">
          <input v-model="newProvider.email" class="form-control" placeholder="Email" required />
        </div>
        <div class="col-md-3">
          <input v-model="newProvider.iban" class="form-control" placeholder="IBAN" required />
        </div>
        <div class="col-md-3">
          <input v-model="newProvider.bic" class="form-control" placeholder="BIC" required />
        </div>
        <div class="col-md-3">
          <input v-model="newProvider.unp" class="form-control" placeholder="УНП" required />
        </div>
        <div class="col-md-3">
          <button type="submit" class="btn btn-success w-100">Добавить</button>
        </div>
      </form>

      <!-- таблица -->
      <table class="table table-bordered table-hover">
        <thead>
          <tr>
            <th>ID</th>
            <th>Название</th>
            <th>Контактное лицо</th>
            <th>Email</th>
            <th>Телефон</th>
            <th>IBAN</th>
            <th>BIC</th>
            <th>УНП</th>
            <th>Действия</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="p in providers" :key="p.providerID">
            <td>{{ p.providerID }}</td>
            <td>
              <span v-if="!p.editing">{{ p.providerName }}</span>
              <input v-else v-model="p.providerName" class="form-control" />
            </td>
            <td>
              <span v-if="!p.editing">{{ p.contactPerson }}</span>
              <input v-else v-model="p.contactPerson" class="form-control" />
            </td>
            <td>
              <span v-if="!p.editing">{{ p.email }}</span>
              <input v-else v-model="p.email" class="form-control" />
            </td>
            <td>
              <span v-if="!p.editing">{{ p.phoneNumber }}</span>
              <input v-else v-model="p.phoneNumber" class="form-control" />
            </td>
            <td>
              <span v-if="!p.editing">{{ p.iban }}</span>
              <input v-else v-model="p.iban" class="form-control" />
            </td>
            <td>
              <span v-if="!p.editing">{{ p.bic }}</span>
              <input v-else v-model="p.bic" class="form-control" />
            </td>
            <td>
              <span v-if="!p.editing">{{ p.unp }}</span>
              <input v-else v-model="p.unp" class="form-control" />
            </td>
            <td>
              <button v-if="!p.editing" @click="p.editing = true" class="btn btn-warning btn-sm">Редактировать</button>
              <button v-else @click="updateProvider(p)" class="btn btn-success btn-sm">Сохранить</button>
              <button @click="deleteProvider(p.providerID)" class="btn btn-danger btn-sm">Удалить</button>
            </td>
          </tr>
          <tr v-if="providers.length === 0">
            <td colspan="9" class="text-center text-muted">Нет провайдеров</td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>
