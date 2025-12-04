<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'

type Invoice = {
  invoiceID: number
  invoiceDate: string
  dueDate: string
  totalAmount: number
  isPaid: boolean
  contractNumber: string
}

const invoices = ref<Invoice[]>([])
const loading = ref(true)
const error = ref<string | null>(null)
const filterStatus = ref<'all' | 'paid' | 'unpaid'>('all')

async function loadInvoices() {
  try {
    const res = await fetch(import.meta.env.VITE_API_URL + '/Invoices')
    if (!res.ok) throw new Error(`Ошибка загрузки: ${res.status}`)
    invoices.value = await res.json()
  } catch (err: any) {
    error.value = err.message
  } finally {
    loading.value = false
  }
}

async function confirmPayment(invoice: Invoice) {
  invoice.isPaid = true
  await fetch(import.meta.env.VITE_API_URL + `/Invoices/${invoice.invoiceID}`, {
    method: 'PUT',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(invoice)
  })
}

// фильтрованные счета
const filteredInvoices = computed(() => {
  if (filterStatus.value === 'paid') {
    return invoices.value.filter(i => i.isPaid)
  } else if (filterStatus.value === 'unpaid') {
    return invoices.value.filter(i => !i.isPaid)
  }
  return invoices.value
})

// отчёт
const report = computed(() => {
  const paidCount = invoices.value.filter(i => i.isPaid).length
  const unpaidCount = invoices.value.filter(i => !i.isPaid).length
  const totalSum = invoices.value.reduce((sum, i) => sum + i.totalAmount, 0)
  return { paidCount, unpaidCount, totalSum }
})

onMounted(loadInvoices)
</script>

<template>
  <div class="page">
    <h2>Управление счетами</h2>

    <div v-if="error" class="error">{{ error }}</div>
    <div v-if="loading">Загрузка...</div>
    <div v-else>
      <!-- фильтр -->
      <div class="filters">
        <label>Фильтр:</label>
        <select v-model="filterStatus">
          <option value="all">Все</option>
          <option value="paid">Оплаченные</option>
          <option value="unpaid">Неоплаченные</option>
        </select>
      </div>

      <!-- отчёт -->
      <div class="report">
        <p>Оплаченные: {{ report.paidCount }}</p>
        <p>Неоплаченные: {{ report.unpaidCount }}</p>
        <p>Общая сумма: {{ report.totalSum }} ₽</p>
      </div>

      <!-- таблица -->
      <table>
        <thead>
          <tr>
            <th>ID</th><th>Дата выставления</th><th>Срок оплаты</th>
            <th>Сумма</th><th>Статус</th><th>Договор</th><th>Действия</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="i in filteredInvoices" :key="i.invoiceID">
            <td>{{ i.invoiceID }}</td>
            <td>{{ new Date(i.invoiceDate).toLocaleDateString('ru-RU') }}</td>
            <td>{{ new Date(i.dueDate).toLocaleDateString('ru-RU') }}</td>
            <td>{{ i.totalAmount }} ₽</td>
            <td>{{ i.isPaid ? 'Оплачен' : 'Не оплачен' }}</td>
            <td>{{ i.contractNumber }}</td>
            <td>
              <button v-if="!i.isPaid" @click="confirmPayment(i)">Подтвердить оплату</button>
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
.filters { margin-bottom: 15px; }
.report { margin-bottom: 15px; background: #f5f5f5; padding: 10px; border-radius: 5px; }
table { width: 100%; border-collapse: collapse; }
th, td { border: 1px solid #ddd; padding: 8px; }
th { background: #f5f5f5; }
button { padding: 5px 10px; cursor: pointer; }
</style>
