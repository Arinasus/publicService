<!-- MyInvoices.vue -->
<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { apiFetch } from '../../services/apiFetch'
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
  contractNumber: string
  addressLine?: string
}

const invoices = ref<Invoice[]>([])
const loading = ref(true)
const error = ref<string | null>(null)
const debugInfo = ref<any>(null)

const selectedInvoice = ref<Invoice | null>(null)

onMounted(async () => {
  await loadInvoices()
})

async function loadInvoices() {
  try {
    loading.value = true
    error.value = null
    debugInfo.value = null

    // Проверяем наличие токена
    const rawAuth = localStorage.getItem('auth')
    if (!rawAuth) {
      error.value = 'Вы не авторизованы. Пожалуйста, войдите в систему.'
      loading.value = false
      return
    }

    const auth = JSON.parse(rawAuth)
    if (!auth.token) {
      error.value = 'Токен не найден. Пожалуйста, войдите снова.'
      loading.value = false
      return
    }

    console.log('Токен для запроса:', auth.token.substring(0, 30) + '...')

    const res = await apiFetch('/Invoices/me')
    debugInfo.value = {
      status: res.status,
      statusText: res.statusText,
      headers: Object.fromEntries(res.headers.entries())
    }

    if (!res.ok) {
      const errorText = await res.text()
      console.error('Ошибка от сервера:', errorText)
      
      if (res.status === 401) {
        error.value = 'Ошибка авторизации. Пожалуйста, войдите снова.'
        localStorage.removeItem('auth')
        setTimeout(() => window.location.href = '/login', 2000)
      } else if (res.status === 403) {
        error.value = 'Доступ запрещен.'
      } else if (res.status === 404) {
        error.value = 'Счетов не найдено.'
        invoices.value = []
      } else {
        error.value = `Ошибка сервера: ${res.status} - ${errorText}`
      }
      return
    }

    const data = await res.json()
    console.log('Полученные счета:', data)
    invoices.value = data

  } catch (err: any) {
    console.error('Ошибка при загрузке счетов:', err)
    
    if (err.message.includes('Failed to fetch')) {
      error.value = 'Не удалось подключиться к серверу. Проверьте:'
      error.value += '\n1. Сервер запущен на localhost:5000'
      error.value += '\n2. Настройки CORS на сервере'
      error.value += '\n3. Сетевое подключение'
    } else {
      error.value = `Ошибка: ${err.message}`
    }
  } finally {
    loading.value = false
  }
}

function openPaymentForm(invoice: Invoice) {
  selectedInvoice.value = invoice
}

function backToInvoices() {
  selectedInvoice.value = null
  loadInvoices() // Перезагружаем счета после оплаты
}
</script>

<template>
  <div class="page">
    <h2>Мои счета</h2>

    <div v-if="error" class="error">
      <h4>Ошибка:</h4>
      <pre>{{ error }}</pre>
      <button @click="loadInvoices" class="retry-btn">Повторить</button>
    </div>

    <div v-if="loading" class="loading">Загрузка счетов...</div>

    <!-- Список счетов -->
    <div v-if="!selectedInvoice && !loading && !error">
      <div v-if="invoices.length === 0" class="no-invoices">
        <p>У вас пока нет счетов. Счета создаются автоматически при подключении услуг.</p>
        <p>Создайте контракт, чтобы получить первый счет.</p>
      </div>
      
      <div v-else class="invoices-container">
        <h3>Всего счетов: {{ invoices.length }}</h3>
        <table class="invoices-table">
          <thead>
            <tr>
              <th>№ счета</th>
              <th>Контракт</th>
              <th>Период</th>
              <th>Сумма</th>
              <th>Статус</th>
              <th>Дата выставления</th>
              <th>Срок оплаты</th>
              <th>Действия</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="inv in invoices" :key="inv.invoiceID">
              <td>{{ inv.invoiceID }}</td>
              <td>{{ inv.contractNumber }}</td>
              <td>{{ inv.period }}</td>
              <td>{{ inv.totalAmount.toFixed(2) }} BYN</td>
              <td>
                <span :class="inv.isPaid ? 'badge paid' : 'badge unpaid'">
                  {{ inv.isPaid ? 'Оплачен' : 'Не оплачен' }}
                </span>
              </td>
              <td>{{ new Date(inv.issueDate).toLocaleDateString('ru-RU') }}</td>
              <td>{{ new Date(inv.dueDate).toLocaleDateString('ru-RU') }}</td>
              <td>
                <div class="actions">
                  <button 
                    v-if="!inv.isPaid" 
                    @click="openPaymentForm(inv)"
                    class="pay-btn"
                  >
                    Оплатить
                  </button>
                  <button 
                    @click="openPaymentForm(inv)"
                    class="details-btn"
                  >
                    Подробности
                  </button>
                </div>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <!-- Wizard оплаты -->
    <PaymentWizard
      v-if="selectedInvoice"
      :invoice="selectedInvoice"
      @back="backToInvoices"
      @payment-success="backToInvoices"
    />
  </div>
</template>

<style scoped>
.page {
  padding: 30px;
  max-width: 1200px;
  margin: 0 auto;
}

/* Заголовок */
h2 {
  font-size: 2rem;
  font-weight: 700;
  color: var(--color-navy);
  margin-bottom: 25px;
}

/* Ошибки */
.error {
  background: #fbe7e9;
  color: var(--color-accent);
  padding: 20px;
  border-radius: var(--radius-md);
  border-left: 4px solid var(--color-accent);
  margin-bottom: 25px;
}

.retry-btn {
  margin-top: 10px;
  padding: 10px 18px;
  border-radius: var(--radius-md);
  background: var(--color-navy);
  color: white;
  border: none;
  font-weight: 600;
  transition: var(--transition);
}

.retry-btn:hover {
  background: #3a4569;
  transform: translateY(-2px);
}

/* Загрузка */
.loading {
  text-align: center;
  padding: 40px;
  color: var(--color-muted);
  font-size: 1.2rem;
}

/* Нет счетов */
.no-invoices {
  background: white;
  padding: 40px;
  border-radius: var(--radius-lg);
  box-shadow: var(--shadow-sm);
  text-align: center;
  color: var(--color-muted);
  margin-top: 20px;
}

/* Контейнер */
.invoices-container {
  margin-top: 20px;
}

/* Таблица */
.invoices-table {
  width: 100%;
  border-collapse: separate;
  border-spacing: 0;
  border-radius: var(--radius-lg);
  overflow: hidden;
  background: white;
  box-shadow: var(--shadow-sm);
}

.invoices-table thead th {
  background: var(--color-navy);
  color: white;
  padding: 14px 18px;
  font-weight: 600;
  font-size: 0.9rem;
  letter-spacing: 0.3px;
}

.invoices-table tbody td {
  padding: 14px 18px;
  border-bottom: 1px solid #eee;
  font-size: 0.95rem;
}

.invoices-table tbody tr:hover {
  background: rgba(47, 54, 80, 0.05);
}

/* Статусы */
.badge {
  padding: 6px 14px;
  border-radius: 20px;
  font-weight: 600;
  font-size: 0.8rem;
}

.badge.paid {
  background: #e6f4ea;
  color: #2f5d3a;
}

.badge.unpaid {
  background: #fbe7e9;
  color: var(--color-accent);
}

/* Кнопки действий */
.actions {
  display: flex;
  gap: 10px;
}

.pay-btn,
.details-btn {
  padding: 8px 14px;
  border-radius: var(--radius-md);
  border: none;
  font-weight: 600;
  cursor: pointer;
  transition: var(--transition);
}

.pay-btn {
  background: #2f5d3a;
  color: white;
}

.pay-btn:hover {
  background: #3c6f47;
  transform: translateY(-2px);
}

.details-btn {
  background: var(--color-navy);
  color: white;
}

.details-btn:hover {
  background: #3a4569;
  transform: translateY(-2px);
}

</style>