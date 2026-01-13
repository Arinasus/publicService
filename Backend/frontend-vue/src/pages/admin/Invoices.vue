<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { apiFetch } from '../../services/apiFetch' 

type Invoice = {
  invoiceID: number
  issueDate: string
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
    const res = await apiFetch('/Invoices')
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
  const res = await apiFetch(`/Invoices/${invoice.invoiceID}`, {   // ✅ заменили fetch
    method: 'PUT',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(invoice)
  })
  if (!res.ok) {
    error.value = `Ошибка подтверждения: ${res.status}`
    invoice.isPaid = false
  }
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
  <link href="https://fonts.googleapis.com/css2?family=Material+Symbols+Outlined" rel="stylesheet"/>
  <div class="page">
    <h2>
      <span class="material-symbols-outlined">receipt_long</span>
      Управление счетами
    </h2>

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
        <p><span class="material-symbols-outlined">done_all</span> Оплаченные: {{ report.paidCount }}</p>
        <p><span class="material-symbols-outlined">hourglass_empty</span> Неоплаченные: {{ report.unpaidCount }}</p>
        <p><span class="material-symbols-outlined">payments</span> Общая сумма: {{ report.totalSum }} BYN</p>
      </div>

      <!-- таблица -->
      <table class="contracts">
        <thead>
          <tr>
            <th>ID</th>
            <th>Дата выставления</th>
            <th>Срок оплаты</th>
            <th>Сумма</th>
            <th>Статус</th>
            <th>Договор</th>
            <th>Действия</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="i in filteredInvoices" :key="i.invoiceID">
            <td>{{ i.invoiceID }}</td>
            <td>{{ new Date(i.issueDate).toLocaleDateString('ru-RU') }}</td>
            <td>{{ new Date(i.dueDate).toLocaleDateString('ru-RU') }}</td>
            <td>{{ i.totalAmount }} BYN</td>
            <td>
              <span :class="i.isPaid ? 'status-paid' : 'status-unpaid'">
                <span class="material-symbols-outlined">
                  {{ i.isPaid ? 'check_circle' : 'error' }}
                </span>
                {{ i.isPaid ? 'Оплачен' : 'Не оплачен' }}
              </span>
            </td>
            <td>{{ i.contractNumber }}</td>
            <td>
              <button v-if="!i.isPaid" @click="confirmPayment(i)" class="btn-confirm">
                <span class="material-symbols-outlined">task_alt</span>
                Подтвердить оплату
              </button>
            </td>
          </tr>
          <tr v-if="filteredInvoices.length === 0">
            <td colspan="7" class="text-muted">Нет счетов по выбранному фильтру</td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>


<style scoped>
.page {
  padding: 30px;
  max-width: 1000px;
  margin: 0 auto;
  font-family: 'Inter', 'Roboto', sans-serif;
}

h2 {
  font-size: 2rem;
  font-weight: 700;
  color: var(--color-navy);
  margin-bottom: 25px;
  display: flex;
  align-items: center;
  gap: 10px;
}

.error {
  background: #fbe7e9;
  color: var(--color-accent);
  padding: 15px;
  border-radius: var(--radius-md);
  border-left: 4px solid var(--color-accent);
  margin-bottom: 20px;
}

.filters {
  margin-bottom: 20px;
  display: flex;
  align-items: center;
  gap: 10px;
}

.filters select {
  padding: 8px;
  border-radius: var(--radius-md);
  border: 1px solid #ccc;
}

.report {
  margin-bottom: 20px;
  background: #f8f9fa;
  padding: 15px;
  border-radius: var(--radius-md);
  box-shadow: var(--shadow-sm);
}

.report p {
  margin: 6px 0;
  display: flex;
  align-items: center;
  gap: 8px;
  font-weight: 500;
}

.contracts {
  width: 100%;
  border-collapse: separate !important;
  border-spacing: 0 !important;
  border: 2px solid var(--color-navy);
  border-radius: 12px;
  overflow: hidden;
  box-shadow: var(--shadow-sm);
}

.contracts th {
  background: var(--color-navy);
  color: white;
  padding: 12px;
  text-align: center;
}

.contracts td {
  padding: 12px;
  border-bottom: 1px solid #eee;
  text-align: center;
}

.contracts tr:hover {
  background: #f9f9f9;
}

.status-paid {
  color: #2f5d3a;
  font-weight: 600;
  display: inline-flex;
  align-items: center;
  gap: 6px;
}

.status-unpaid {
  color: #c62828;
  font-weight: 600;
  display: inline-flex;
  align-items: center;
  gap: 6px;
}

/* Кнопка подтверждения */
.btn-confirm {
  display: inline-flex;
  align-items: center;
  gap: 6px;
  background: #4caf50;
  color: white;
  border: none;
  border-radius: var(--radius-md);
  padding: 8px 14px;
  font-weight: 600;
  cursor: pointer;
  transition: background 0.3s ease, transform 0.2s ease;
}

.btn-confirm:hover {
  background: #388e3c;
  transform: translateY(-1px);
}

</style>
