<script setup lang="ts">
import { ref, onMounted } from 'vue'
type Contract =
{ contractID: number
  contractNumber: string
  contractStartDate: string
  contractEndDate: string
  userName: string
  address: string
  serviceName: string
  providerName: string
}
const contracts = ref<Contract[]>([])
const loading = ref(true)
const error = ref<string | null>(null)
onMounted(async () => {
  try{
    const res = await fetch(import.meta.env.VITE_API_URL + '/Contracts', {
    headers: { 'Content-Type': 'application/json' },
    })
    if(!res.ok) throw new Error(`Ошибка загрузки: ${res.status}`)
    contracts.value = await res.json()
  } catch (err: any)
  {error.value = err.message}
  finally {loading.value = false}
})
</script>

<template>
  <div class="page container py-4">
    <h2 class="text-green-dark mb-4">Мои услуги</h2>

    <div v-if="error" class="alert alert-danger">{{ error }}</div>
    <div v-if="loading" class="text-muted">Загрузка...</div>

    <div v-else class="card shadow-sm">
      <div class="card-body">
        <table class="contracts table table-bordered table-striped table-hover align-middle mb-0">
          <thead class="table-header">
            <tr>
              <th>Номер контракта</th>
              <th>Услуга</th>
              <th>Провайдер</th>
              <th>Адрес</th>
              <th>Срок действия</th>
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
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  </div>
</template>

<style scoped>
:root {
  --color-mint: #E6F4EC;
  --color-green-light: #A3D9A5;
  --color-green-deep: #4B8F6B;
  --color-green-dark: #2F5D3A;
  --color-yellow-green: #C4D96F;
}

.page {
  background-color: var(--color-mint);
  min-height: 100vh;
  font-family: 'Inter', 'Roboto', sans-serif;
}

.text-green-dark {
  color: var(--color-green-dark);
}

/* таблица */
.contracts {
  border: 2px solid var(--color-green-deep);
  border-radius: 8px;
  overflow: hidden;
  background-color: #fff;
}

/* шапка таблицы */
.table-header {
  background-color: var(--color-green-light);
  color: var(--color-green-dark);
  font-weight: 600;
  text-transform: uppercase;
}

/* строки */
.contracts tbody tr:nth-child(even) {
  background-color: var(--color-mint);
}

.contracts tbody tr:hover {
  background-color: var(--color-yellow-green);
  color: var(--color-green-dark);
  transition: 0.3s ease;
}

/* ячейки */
.contracts th, .contracts td {
  text-align: center;
  padding: 12px;
  border: 1px solid var(--color-green-light);
}
</style>
