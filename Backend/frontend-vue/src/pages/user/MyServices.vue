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
  <div class="page">
    <h2>Мои услуги</h2>

    <div v-if="error" class="error">{{ error }}</div>
    <div v-if="loading">Загрузка...</div>

    <table v-else class="contracts">
      <thead>
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
</template>

<style scoped>
.page { padding: 20px; }
.error { color: red; margin-bottom: 10px; }
.contracts { width: 100%; border-collapse: collapse; }
.contracts th, .contracts td { border: 1px solid #ddd; padding: 8px; text-align: center; }
.contracts th { background-color: #f4f4f4; }
</style>