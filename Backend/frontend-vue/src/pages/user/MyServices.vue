<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { apiFetch } from '../../services/apiFetch'

type Contract = {
  contractID: number
  contractNumber: string
  contractStartDate: string
  contractEndDate: string
  userName: string
  address: string
  providerName: string
  services: Array<{
    serviceID: number
    serviceName: string
    unitOfMeasure: string
    price: number
  }>
}

type Service = {
  serviceID: number
  serviceName: string
  unitOfMeasure: string
  price: number
  providerID: number
  providerName: string
  isAvailable?: boolean
}

const contracts = ref<Contract[]>([])
const availableServices = ref<Service[]>([]) // переименовали с services на availableServices
const loading = ref(true)
const error = ref<string | null>(null)

onMounted(async () => {
  try {
    // контракты пользователя
    const resContracts = await apiFetch('/Contracts/me')
    if (!resContracts.ok) throw new Error(`Ошибка загрузки контрактов: ${resContracts.status}`)
    contracts.value = await resContracts.json()

    // ДОСТУПНЫЕ услуги (которые пользователь еще не подключил)
    const resServices = await apiFetch('/Services/available') // ИЗМЕНИЛИ эндпоинт
    if (!resServices.ok) throw new Error(`Ошибка загрузки услуг: ${resServices.status}`)
    availableServices.value = await resServices.json()
    
  } catch (err: any) {
    error.value = err.message
  } finally {
    loading.value = false
  }
})

// Функция для проверки, подключена ли услуга
function isServiceSubscribed(serviceID: number): boolean {
  return contracts.value.some(contract => 
    contract.services?.some(service => service.serviceID === serviceID)
  )
}

async function addService(serviceID: number) {
  if (isServiceSubscribed(serviceID)) {
    alert('Вы уже подписаны на эту услугу!')
    return
  }

  const service = availableServices.value.find(s => s.serviceID === serviceID)
  if (!service) {
    alert('Услуга не найдена!')
    return
  }

  const resProfile = await apiFetch('/Users/me') 
  if (!resProfile.ok) { 
    alert('Ошибка получения профиля') 
    return 
  } 
  const profile = await resProfile.json()

  const addressID = profile.addresses?.[0]?.addressID
  if (!addressID) {
    alert('У пользователя нет адреса!')
    return
  }

  const contractPayload = {
    providerID: service.providerID,
    addressID: addressID,
    contractNumber: 'CNT-' + Date.now(),
    contractStartDate: new Date().toISOString(),
    contractEndDate: new Date(Date.now() + 30*24*60*60*1000).toISOString(),
    serviceIds: [service.serviceID]
  }

  const res = await apiFetch('/Contracts', {
    method: 'POST', 
    headers: { 'Content-Type': 'application/json' }, 
    body: JSON.stringify(contractPayload) 
  })
  
  if (!res.ok) {
    const errorText = await res.text()
    throw new Error(`Ошибка добавления: ${res.status} - ${errorText}`)
  }
  
  // Обновляем списки после успешного добавления
  const newContract = await res.json()
  contracts.value.push(newContract)
  
  // Убираем добавленную услугу из списка доступных
  availableServices.value = availableServices.value.filter(s => s.serviceID !== serviceID)
}

async function removeService(contractID: number) { 
  if (!confirm('Вы уверены, что хотите отказаться от услуги?')) return 
  
  await apiFetch(`/Contracts/${contractID}`, { method: 'DELETE' }) 
  
  // Обновляем список контрактов
  contracts.value = contracts.value.filter(c => c.contractID !== contractID)
  
  // Нужно перезагрузить доступные услуги
  const resServices = await apiFetch('/Services/available')
  if (resServices.ok) {
    availableServices.value = await resServices.json()
  }
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
              <th>Услуги</th>
              <th>Провайдер</th>
              <th>Адрес</th>
              <th>Срок действия</th>
              <th>Действие</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="c in contracts" :key="c.contractID">
              <td>{{ c.contractNumber }}</td>
              <td>
                <div v-if="c.services && c.services.length > 0">
                  <div v-for="service in c.services.slice(0, 2)" :key="service.serviceID">
                    {{ service.serviceName }}
                  </div>
                  <span v-if="c.services.length > 2" class="text-muted small">
                    +{{ c.services.length - 2 }} еще
                  </span>
                </div>
                <span v-else class="text-muted">Нет услуг</span>
              </td>
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

    <!-- список ДОСТУПНЫХ услуг -->
    <h2 class="text-green-dark mb-4">Доступные услуги</h2>
    <div class="card shadow-sm">
      <div class="card-body">
        <table class="contracts table table-bordered table-hover align-middle mb-0">
          <thead class="table-header">
            <tr>
              <th>Название</th>
              <th>Ед. изм.</th>
              <th>Цена</th>
              <th>Провайдер</th>
              <th>Действие</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="s in availableServices" :key="s.serviceID">
              <td>{{ s.serviceName }}</td>
              <td>{{ s.unitOfMeasure }}</td>
              <td>{{ s.price }} ₽</td>
              <td>{{ s.providerName }}</td>
              <td>
                <!-- Кнопка всегда активна, так как это список доступных услуг -->
                <button
                  @click="addService(s.serviceID)"
                  class="btn btn-success btn-sm"
                >
                  Добавить
                </button>
              </td>
            </tr>
            <tr v-if="availableServices.length === 0">
              <td colspan="5" class="text-center text-muted">
                Все доступные услуги уже подключены
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  </div>
</template>