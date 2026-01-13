<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { apiFetch } from '../../services/apiFetch'

type Service = {
  serviceID: number
  serviceName: string
  unitOfMeasure: string
  price: number
  providerID: number
  providerName?: string
  editing?: boolean
}

type Provider = {
  providerID: number
  providerName: string
}

const services = ref<Service[]>([])
const providers = ref<Provider[]>([])
const loading = ref(true)
const error = ref<string | null>(null)

const newService = ref<Omit<Service, 'serviceID' | 'providerName'>>({
  serviceName: '',
  unitOfMeasure: '',
  price: 0,
  providerID: 0
})

async function loadProviders() {
  try {
    const res = await apiFetch('/Providers')
    if (!res.ok) throw new Error(`Ошибка загрузки провайдеров: ${res.status}`)
    const data = await res.json()
    providers.value = Array.isArray(data) ? data : []
    // Устанавливаем первого провайдера по умолчанию
if (providers.value.length > 0) {
  newService.value.providerID = providers.value[0]?.providerID ?? 0
}

  } catch (err: any) {
    console.error('Ошибка загрузки провайдеров:', err)
  }
}

async function loadServices() {
  try {
    const res = await apiFetch('/Services')
    if (!res.ok) throw new Error(`Ошибка загрузки: ${res.status}`)
    const data = await res.json()
    services.value = Array.isArray(data) ? data : []
  } catch (err: any) {
    error.value = err.message
  } finally {
    loading.value = false
  }
}

async function addService() {
  try {
    // Валидация
    if (!newService.value.serviceName?.trim()) {
      error.value = 'Введите название услуги'
      return
    }
    if (!newService.value.unitOfMeasure?.trim()) {
      error.value = 'Введите единицу измерения'
      return
    }
    if (newService.value.price <= 0) {
      error.value = 'Цена должна быть больше 0'
      return
    }
    if (!newService.value.providerID) {
      error.value = 'Выберите провайдера'
      return
    }

    const res = await apiFetch('/Services', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({
        serviceName: newService.value.serviceName.trim(),
        unitOfMeasure: newService.value.unitOfMeasure.trim(),
        price: newService.value.price,
        providerID: newService.value.providerID
      })
    })
    
    if (!res.ok) {
      const errorText = await res.text()
      throw new Error(`Ошибка добавления: ${res.status} - ${errorText}`)
    }
    
    const created: Service = await res.json()
    services.value.push(created)
    
    // Сброс формы
    newService.value = { 
      serviceName: '', 
      unitOfMeasure: '', 
      price: 0,
      providerID: providers.value.length > 0 ? providers.value[0]!.providerID : 0
    }
    error.value = null
    
  } catch (err: any) {
    error.value = err.message
  }
}

async function updateService(service: Service) {
  try {
    const res = await apiFetch(`/Services/${service.serviceID}`, {
      method: 'PUT',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({
        serviceID: service.serviceID,
        serviceName: service.serviceName,
        unitOfMeasure: service.unitOfMeasure,
        price: service.price,
        providerID: service.providerID
      })
    })
    
    if (!res.ok) {
      const errorText = await res.text()
      throw new Error(`Ошибка обновления: ${res.status} - ${errorText}`)
    }
    
    service.editing = false
    error.value = null
    
  } catch (err: any) {
    error.value = err.message
  }
}

async function deleteService(id: number) {
  if (!confirm('Вы уверены, что хотите удалить услугу?')) return
  
  try {
    const res = await apiFetch(`/Services/${id}`, { method: 'DELETE' })
    
    if (!res.ok) {
      const errorText = await res.text()
      throw new Error(`Ошибка удаления: ${res.status} - ${errorText}`)
    }
    
    services.value = services.value.filter(s => s.serviceID !== id)
    error.value = null
    
  } catch (err: any) {
    error.value = err.message
  }
}

function getProviderName(providerID: number): string {
  const provider = providers.value.find(p => p.providerID === providerID)
  return provider ? provider.providerName : 'Неизвестно'
}

onMounted(async () => {
  try {
    await loadProviders()
    await loadServices()
  } catch (err: any) {
    error.value = err.message
  } finally {
    loading.value = false
  }
})
</script>
<template>
  <link href="https://fonts.googleapis.com/css2?family=Material+Symbols+Outlined" rel="stylesheet"/>
  <div class="page container py-4">
    <h2 class="text-green-dark mb-4">
      <span class="material-symbols-outlined">build</span>
      Управление услугами
    </h2>

    <div v-if="error" class="alert alert-danger">{{ error }}</div>
    <div v-else-if="loading" class="text-muted">Загрузка...</div>
    <div v-else>
      <!-- форма добавления -->
      <form @submit.prevent="addService" class="row g-2 mb-4">
        <div class="col-md-2">
          <input v-model="newService.serviceName" class="form-control" placeholder="Название" required />
        </div>
        <div class="col-md-2">
          <input v-model="newService.unitOfMeasure" class="form-control" placeholder="Ед. изм." required />
        </div>
        <div class="col-md-2">
          <input v-model.number="newService.price" type="number" min="0" step="0.01" class="form-control" placeholder="Цена" required />
        </div>
        <div class="col-md-3">
          <select v-model.number="newService.providerID" class="form-select" required>
            <option value="0" disabled>Выберите провайдера</option>
            <option v-for="provider in providers" :key="provider.providerID" :value="provider.providerID">
              {{ provider.providerName }}
            </option>
          </select>
        </div>
        <div class="col-md-3">
  <button type="submit" class="btn-add w-100">
    <span class="material-symbols-outlined">add_circle</span>
    Добавить услугу
  </button>
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
            <th>Провайдер</th>
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
              <span v-if="!s.editing">{{ s.price }} BYN</span>
              <input v-else type="number" v-model.number="s.price" min="0" step="0.01" class="form-control" />
            </td>
            <td>
              <span v-if="!s.editing">{{ getProviderName(s.providerID) }}</span>
              <select v-else v-model.number="s.providerID" class="form-select">
                <option value="0" disabled>Выберите провайдера</option>
                <option v-for="provider in providers" :key="provider.providerID" :value="provider.providerID">
                  {{ provider.providerName }}
                </option>
              </select>
            </td>
            <td>
  <div class="actions">
    <button v-if="!s.editing" @click="s.editing = true" class="btn-edit btn-sm">
      <span class="material-symbols-outlined">edit</span> Редактировать
    </button>
    <button v-else @click="updateService(s)" class="btn-save btn-sm">
      <span class="material-symbols-outlined">save</span> Сохранить
    </button>
    <button @click="deleteService(s.serviceID)" class="btn-delete btn-sm">
      <span class="material-symbols-outlined">delete</span> Удалить
    </button>
  </div>
</td>
          </tr>
          <tr v-if="services.length === 0">
            <td colspan="6" class="text-center text-muted">
              Нет услуг. Добавьте первую услугу.
            </td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>


<style scoped>
.contracts {
  border: 2px solid var(--color-green-deep);
  border-radius: 12px; /* чуть больше радиус */
  overflow: hidden;
  background-color: var(--color-lemon);
  box-shadow: 0 4px 10px rgba(0, 0, 0, 0.1);
  margin: 20px auto; /* центрируем */
  width: 100%;
}

.contracts th,
.contracts td {
  text-align: center;
  padding: 12px;
  border: 1px solid var(--color-green-deep);
}

/* убираем конфликт с bootstrap */
.table {
  border-collapse: separate !important;
  border-spacing: 0 !important;
}

.btn-add {
  display: inline-flex;              /* иконка + текст в одну линию */
  align-items: center;               /* вертикальное выравнивание */
  justify-content: center;           /* центрируем содержимое */
  gap: 8px;                          /* отступ между иконкой и текстом */
  background: linear-gradient(135deg, var(--color-navy), #3a4569);
  color: white;
  border-radius: var(--radius-md);
  font-weight: 600;
  padding: 12px 20px;
  border: none;
  cursor: pointer;
  transition: background 0.3s ease, box-shadow 0.3s ease, transform 0.2s ease;
}

.btn-add .material-symbols-outlined {
  font-size: 22px;                   /* чуть больше иконка */
  line-height: 1;                    /* убираем лишние отступы */
}

.btn-add:hover {
  background: #2f3650;
  box-shadow: 0 4px 12px rgba(0,0,0,0.2);
  transform: translateY(-1px);       /* лёгкий подъём, не дерганый */
}


/* Кнопки действий */
.actions {
  display: flex;
  gap: 12px; /* больше отступ между кнопками */
  justify-content: center;
}

.btn-edit,
.btn-save,
.btn-delete {
  display: inline-flex;
  align-items: center;
  gap: 6px;
  padding: 8px 14px;
  border-radius: var(--radius-md);
  font-weight: 600;
  cursor: pointer;
  border: none;
  transition: background 0.3s ease, transform 0.2s ease;
}

.btn-edit .material-symbols-outlined,
.btn-save .material-symbols-outlined,
.btn-delete .material-symbols-outlined {
  font-size: 18px;
  line-height: 1;
}

.btn-edit { background: #ff9800; color: white; }
.btn-edit:hover { background: #f57c00; transform: translateY(-1px); }

.btn-save { background: #4caf50; color: white; }
.btn-save:hover { background: #388e3c; transform: translateY(-1px); }

.btn-delete { background: #f44336; color: white; }
.btn-delete:hover { background: #d32f2f; transform: translateY(-1px); }


</style>