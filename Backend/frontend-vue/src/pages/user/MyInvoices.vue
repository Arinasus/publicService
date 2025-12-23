<script setup lang="ts">
import { ref, onMounted } from 'vue'
import PaymentWizard from './PaymentWizard.vue'

type Invoice = {
  invoiceID: number
  contractID: number
  issueDate: string
  dueDate: string
  period: string
  totalAmount: number
  isPaid: boolean
  paymentDate?: string
}

const invoices = ref<Invoice[]>([])
const loading = ref(true)
const error = ref<string | null>(null)

const selectedInvoice = ref<Invoice | null>(null)

onMounted(async () => {
  try {
    const res = await fetch(import.meta.env.VITE_API_URL + '/Invoices', {
      headers: { 'Content-Type': 'application/json' },
    })
    if (!res.ok) throw new Error(`Ошибка загрузки: ${res.status}`)
    invoices.value = await res.json()
  } catch (err: any) {
    error.value = err.message
  } finally {
    loading.value = false
  }
})

function openPaymentForm(invoice: Invoice) {
  selectedInvoice.value = invoice
}

function backToInvoices() {
  selectedInvoice.value = null
}
</script>

<template>
  <div class="page">
    <h2>Мои счета</h2>

    <div v-if="error" class="error">{{ error }}</div>
    <div v-if="loading">Загрузка...</div>

    <!-- Список счетов -->
    <table v-if="!selectedInvoice" class="invoices">
      <thead>
        <tr>
          <th>ID</th><th>Сумма</th><th>Статус</th><th>Срок оплаты</th><th>Действие</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="inv in invoices" :key="inv.invoiceID">
          <td>{{ inv.invoiceID }}</td>
          <td>{{ inv.totalAmount.toFixed(2) }} BYN</td>
          <td><span :class="inv.isPaid ? 'paid' : 'unpaid'">{{ inv.isPaid ? 'Оплачен' : 'Не оплачен' }}</span></td>
          <td>{{ new Date(inv.issueDate).toLocaleDateString('ru-RU') }} – {{ new Date(inv.dueDate).toLocaleDateString('ru-RU') }}</td>
          <td><button v-if="!inv.isPaid" @click="openPaymentForm(inv)">Оплатить</button></td>
        </tr>
      </tbody>
    </table>

    <!-- Wizard оплаты -->
    <PaymentWizard
      v-if="selectedInvoice"
      :invoice="selectedInvoice"
      @back="backToInvoices"
    />
  </div>
</template>

<style scoped>
.page { padding: 20px; }
.error { color: red; margin-bottom: 10px; }
.invoices { width: 100%; border-collapse: collapse; }
.invoices th, .invoices td { border: 1px solid #ddd; padding: 8px; text-align: center; }
.invoices th { background-color: #f4f4f4; }
.paid { color: green; font-weight: bold; }
.unpaid { color: red; font-weight: bold; }
button { padding: 6px 12px; background-color: #0078d7; color: white; border: none; border-radius: 4px; cursor: pointer; }
button:hover { background-color: #005a9e; }
</style>
