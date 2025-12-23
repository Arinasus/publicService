<script setup lang="ts">
import { ref, onMounted } from 'vue'

type Contract = {
  contractID: number
  contractNumber: string
  contractStartDate: string
  contractEndDate: string
  userName: string
  address: string
  serviceName: string
  providerName: string
  serviceID?: number   // добавим связь с услугой
}
// type ContractCreate = {
//   userID: number
//   addressID: number
//   serviceID: number
//   providerID: number
//   contractNumber: string
//   contractStartDate: string
//   contractEndDate: string
// }
type Service = {
  serviceID: number
  serviceName: string
  unitOfMeasure: string
  price: number
  providerID: number 
}

const contracts = ref<Contract[]>([])
const services = ref<Service[]>([])
const loading = ref(true)
const error = ref<string | null>(null)

onMounted(async () => {
  try {
    // контракты пользователя
    const resContracts = await fetch(import.meta.env.VITE_API_URL + '/Contracts')
    if (!resContracts.ok) throw new Error(`Ошибка загрузки контрактов: ${resContracts.status}`)
    contracts.value = await resContracts.json()

    // все доступные услуги
    const resServices = await fetch(import.meta.env.VITE_API_URL + '/Services')
    if (!resServices.ok) throw new Error(`Ошибка загрузки услуг: ${resServices.status}`)
    services.value = await resServices.json()
  } catch (err: any) {
    error.value = err.message
  } finally {
    loading.value = false
  }
})

async function addService(serviceID: number) {
  const alreadySubscribed = contracts.value.some(c => c.serviceID === serviceID)
  if (alreadySubscribed) {
    alert('Вы уже подписаны на эту услугу!')
    return
  }

  const service = services.value.find(s => s.serviceID === serviceID)
  if (!service) {
    alert('Услуга не найдена!')
    return
  }

  const token = localStorage.getItem('token')
  const profile = JSON.parse(localStorage.getItem('profile') || '{}')
  if (!token || !profile.userID) {
    alert('Вы не авторизованы!')
    return
  }

  const addressID = profile.addresses?.[0]?.addressID
  if (!addressID) {
    alert('У пользователя нет адреса!')
    return
  }

  const contractPayload = {
    serviceID: service.serviceID,
    providerID: service.providerID,
    addressID: addressID,
    contractNumber: 'CNT-' + Date.now(),
    contractStartDate: new Date().toISOString(),
    contractEndDate: new Date(Date.now() + 30*24*60*60*1000).toISOString()
  }

  const res = await fetch(import.meta.env.VITE_API_URL + '/Contracts', {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
      'Authorization': 'Bearer ' + token
    },
    body: JSON.stringify(contractPayload)
  })
  if (!res.ok) throw new Error(`Ошибка добавления: ${res.status}`)
  const newContract = await res.json()
  contracts.value.push(newContract)
}


async function removeService(contractID: number) {
  if (!confirm('Вы уверены, что хотите отказаться от услуги?')) return
  await fetch(import.meta.env.VITE_API_URL + `/Contracts/${contractID}`, { method: 'DELETE' })
  contracts.value = contracts.value.filter(c => c.contractID !== contractID)
}
</script>

<template>
  <div class="page container py-4">
    <h2 class="text-green-dark mb-4">Мои услуги</h2>

    <div v-if="error" class="alert alert-danger">{{ error }}</div>
    <div v-else-if="loading" class="text-muted">Загрузка...</div>

    <!-- таблица контрактов пользователя -->
    <div v-else class="card shadow-sm mb-4">
      <div class="card-body">
        <table class="contracts table table-bordered table-striped table-hover align-middle mb-0">
          <thead class="table-header">
            <tr>
              <th>Номер контракта</th>
              <th>Услуга</th>
              <th>Провайдер</th>
              <th>Адрес</th>
              <th>Срок действия</th>
              <th>Действие</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="c in contracts" :key="c.contractID">
              <td>{{ c.contractNumber }}</td>
              <td>{{ c.serviceName }}</td>
              <td>{{ c.providerName }}</td>
              <td>{{ c.address }}</td>
              <td>
                {{ new Date(c.contractStartDate).toLocaleDateString('ru-RU') }} –
                {{ c.contractEndDate ? new Date(c.contractEndDate).toLocaleDateString('ru-RU') : '—' }}
              </td>
              <td>
                <button @click="removeService(c.contractID)" class="btn btn-danger btn-sm">Отказаться</button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <!-- список доступных услуг -->
    <h2 class="text-green-dark mb-4">Доступные услуги</h2>
    <div class="card shadow-sm">
      <div class="card-body">
        <table class="contracts table table-bordered table-hover align-middle mb-0">
          <thead class="table-header">
            <tr>
              <th>Название</th>
              <th>Ед. изм.</th>
              <th>Цена</th>
              <th>Действие</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="s in services" :key="s.serviceID">
              <td>{{ s.serviceName }}</td>
              <td>{{ s.unitOfMeasure }}</td>
              <td>{{ s.price }} ₽</td>
              <td>
                <button
                  v-if="contracts.some(c => c.serviceID === s.serviceID)"
                  class="btn btn-secondary btn-sm" disabled
                >
                  Уже подключена
                </button>
                <button
                  v-else
                  @click="addService(s.serviceID)"
                  class="btn btn-success btn-sm"
                >
                  Добавить
                </button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  </div>
</template>
