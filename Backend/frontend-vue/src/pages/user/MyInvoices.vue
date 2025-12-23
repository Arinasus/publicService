<script setup lang="ts">
import { ref, onMounted } from 'vue'

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
// состояние модального окна 
const showModal = ref(false) 
const selectedInvoice = ref<Invoice | null>(null) 
const paymentMethod = ref("Card")
onMounted(async () => {
  try {
    const res = await fetch(import.meta.env.VITE_API_URL + '/Invoices', {
      headers: { 'Content-Type': 'application/json' },
    })
    if (!res.ok) {
      throw new Error(`Ошибка загрузки: ${res.status}`)
    }
    invoices.value = await res.json()
  } catch (err: any) {
    error.value = err.message
  } finally {
    loading.value = false
  }
})
function openPaymentForm(invoice: Invoice) {
  selectedInvoice.value = invoice
  showModal.value = true
}
// Функция оплаты
async function payInvoice() {
  if (!selectedInvoice.value) return
  try {
    const body = {
      paymentAmount: selectedInvoice.value.totalAmount,
      paymentMethod: paymentMethod.value
    }

    const res = await fetch(
      import.meta.env.VITE_API_URL + `/Invoices/${selectedInvoice.value.invoiceID}/pay`,
      {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(body)
      }
    )

    if (!res.ok) {
      throw new Error(`Ошибка оплаты: ${res.status}`)
    }

    // Получаем PDF как Blob
    const blob = await res.blob()
    const url = window.URL.createObjectURL(blob)

    const link = document.createElement("a")
    link.href = url
    link.download = `receipt_${selectedInvoice.value.invoiceID}.pdf`
    document.body.appendChild(link)
    link.click()
    link.remove()
    window.URL.revokeObjectURL(url)

    // Обновляем статус локально
    selectedInvoice.value.isPaid = true
    showModal.value = false
  } catch (err: any) {
    alert(`Ошибка оплаты: ${err.message}`)
  }
}

</script>

<template>
  <div class="page">
    <h2>Мои счета</h2>

    <div v-if="error" class="error">{{ error }}</div>
    <div v-if="loading">Загрузка...</div>

    <table v-else class="invoices">
      <thead>
        <tr>
          <th>ID</th>
          <th>Сумма</th>
          <th>Статус</th>
          <th>Срок оплаты</th>
          <th>Действие</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="inv in invoices" :key="inv.invoiceID">
          <td>{{ inv.invoiceID }}</td>
          <td>{{ inv.totalAmount ? inv.totalAmount.toFixed(2) : '—' }} BYN</td>
          <td>
            <span :class="inv.isPaid ? 'paid' : 'unpaid'">
              {{ inv.isPaid ? 'Оплачен' : 'Не оплачен' }}
            </span>
          </td>
          <td>{{ new Date(inv.issueDate).toLocaleDateString('ru-RU') }} – {{ new Date(inv.dueDate).toLocaleDateString('ru-RU') }}</td>
          <td>
            <button v-if="!inv.isPaid" @click="openPaymentForm(inv)">Оплатить</button>
            <span v-else>—</span>
          </td>
        </tr>
      </tbody>
    </table>

    <!-- Модальное окно -->
    <div v-if="showModal" class="modal">
      <div class="modal-content">
        <h3>Оплата счёта №{{ selectedInvoice?.invoiceID }}</h3>
        <p>Период: {{ selectedInvoice?.period }}</p>
        <p>Сумма: {{ selectedInvoice?.totalAmount.toFixed(2) }} BYN</p>
        <p>Срок оплаты: {{ new Date(selectedInvoice?.issueDate!).toLocaleDateString('ru-RU') }} – {{ new Date(selectedInvoice?.dueDate!).toLocaleDateString('ru-RU') }}</p>

        <label>
          Способ оплаты:
          <select v-model="paymentMethod">
            <option value="Card">Карта</option>
            <option value="Cash">Наличные</option>
          </select>
        </label>

        <div class="actions">
          <button @click="payInvoice">Подтвердить оплату</button>
          <button @click="showModal = false">Отмена</button>
        </div>
      </div>
    </div>
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

/* Модальное окно */
.modal {
  position: fixed;
  top: 0; left: 0; right: 0; bottom: 0;
  background: rgba(0,0,0,0.5);
  display: flex; align-items: center; justify-content: center;
}
.modal-content {
  background: white;
  padding: 20px;
  border-radius: 8px;
  width: 400px;
}
.actions {
  margin-top: 20px;
  display: flex;
  justify-content: space-between;
}
</style>
