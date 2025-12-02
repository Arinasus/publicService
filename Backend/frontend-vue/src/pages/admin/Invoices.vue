<script setup lang="ts">
import { ref, onMounted } from 'vue'

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

async function markPaid(id: number) {
  await fetch(import.meta.env.VITE_API_URL + `/Invoices/${id}`, {
    method: 'PUT',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ isPaid: true })
  })
  const invoice = invoices.value.find(i => i.invoiceID === id)
  if (invoice) invoice.isPaid = true
}

onMounted(loadInvoices)
</script>

<template>
  <div class="page">
    <h2>Управление счетами</h2>
    <div v-if="error" class="error">{{ error }}</div>
    <div v-if="loading">Загрузка...</div>

    <table v-else>
      <thead>
        <tr>
          <th>ID</th><th>Пользователь</th><th>Сумма</th><th>Срок оплаты</th><th>Статус</th><th>Действия</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="i in invoices" :key="i.invoiceID">
          <td>{{ i.invoiceID }}</td>
          <td>{{ new Date(i.invoiceDate).toLocaleDateString('ru-RU') }}</td>
          <td>{{ new Date(i.dueDate).toLocaleDateString('ru-RU') }}</td>
          <td>{{ i.totalAmount }} ₽</td>
          <td>{{ i.isPaid ? 'Оплачен' : 'Не оплачен' }}</td>
          <td>{{ i.contractNumber }}</td>
          <td>
            <button v-if="!i.isPaid" @click="markPaid(i.invoiceID)">Подтвердить оплату</button>
          </td>
        </tr>
      </tbody>
    </table>
  </div>
</template>
