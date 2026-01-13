<template>
  <div class="payment-wizard">
    <button class="btn-back" @click="$emit('back')">← Назад к счетам</button>

    
    <!-- Загрузка -->
    <div v-if="loading" class="loading">
      Загрузка данных счета...
    </div>
    
    <!-- Ошибка -->
    <div v-else-if="loadError" class="error">
      {{ loadError }}
    </div>
    
    <!-- Основной контент -->
    <component
      v-else-if="provider && payer"
      :is="currentStepComponent"
      :provider="provider"
      :contractID="invoice.contractID"
      :payer="payer"
      :invoice="invoice"
      :meters="meters"
      :paymentMethod="paymentMethod"
      @next="nextStep"
      @prev="prevStep"
      @update:paymentMethod="updatePaymentMethod"
      @pay="payInvoice"
    />
    
    <!-- Если данных нет -->
    <div v-else class="error">
      Не удалось загрузить данные счета
    </div>

    <!-- Индикатор шагов -->
    <div class="steps-indicator">
      Шаг {{ step }} из 3
    </div>
  </div>
</template>


<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import PaymentStepProvider from './PaymentStepProvider.vue'
import PaymentStepDetails from './PaymentStepDetails.vue'
import PaymentStepConfirm from './PaymentStepConfirm.vue'

const props = defineProps<{
  invoice: {
    invoiceID: number
    contractID: number
    issueDate: string
    dueDate: string
    period: string
    totalAmount: number
    isPaid: boolean
  }
}>()

const emit = defineEmits<{
  (e: 'back'): void
  (e: 'payment-success'): void
}>()

const step = ref(1)
const paymentMethod = ref("Card")

// данные из API
const provider = ref<any>(null)
const payer = ref<any>(null)
const meters = ref<any[]>([])

// состояния загрузки
const loading = ref(true)
const loadError = ref<string | null>(null)
const debugInfo = ref<any>(null)

onMounted(async () => {
  try {
    loading.value = true
    loadError.value = null
    debugInfo.value = null
    
    // Получаем токен из localStorage
    const rawAuth = localStorage.getItem('auth')
    if (!rawAuth) {
      loadError.value = 'Ошибка авторизации. Пожалуйста, войдите снова.'
      loading.value = false
      return
    }
    
    const auth = JSON.parse(rawAuth)
    const token = auth.token
    
    console.log('Загружаем детали счета ID:', props.invoice.invoiceID)
    
    const res = await fetch(
      import.meta.env.VITE_API_URL + `/InvoiceDetails/${props.invoice.invoiceID}`,
      {
        headers: {
          'Authorization': `Bearer ${token}`
        }
      }
    )
    
    debugInfo.value = {
      status: res.status,
      statusText: res.statusText,
      ok: res.ok,
      headers: Object.fromEntries(res.headers.entries())
    }
    
    if (!res.ok) {
      const errorText = await res.text()
      debugInfo.value.body = errorText
      throw new Error(`Ошибка загрузки: ${res.status} - ${errorText}`)
    }
    
    const data = await res.json()
    debugInfo.value.body = data
    console.log("InvoiceDetails response:", data)

    // Проверяем, что данные существуют
    if (!data) {
      throw new Error('Данные счета не получены')
    }
    
    provider.value = data.provider || null
    payer.value = data.payer || null
    meters.value = data.meters || []
    
    console.log('Загруженные данные:', {
      provider: provider.value,
      payer: payer.value,
      meters: meters.value
    })
    
  } catch (err: any) {
    console.error('Ошибка загрузки данных счета:', err)
    loadError.value = `Ошибка загрузки данных: ${err.message}`
  } finally {
    loading.value = false
  }
})

const nextStep = () => { if (step.value < 3) step.value++ }
const prevStep = () => { if (step.value > 1) step.value-- }

const currentStepComponent = computed(() => {
  switch (step.value) {
    case 1: return PaymentStepProvider
    case 2: return PaymentStepDetails
    case 3: return PaymentStepConfirm
    default: return undefined
  }
})

// обновление метода оплаты из дочернего компонента
function updatePaymentMethod(val: string) {
  paymentMethod.value = val
}
// function updateMeters(updatedMeters: any[]) {
//   meters.value = updatedMeters
// }
// оплата
async function payInvoice(method: string) {
  try {
    // Получаем токен
    const rawAuth = localStorage.getItem('auth')
    if (!rawAuth) {
      alert('Ошибка авторизации. Пожалуйста, войдите снова.')
      return
    }
    
    const auth = JSON.parse(rawAuth)
    const token = auth.token
    
    const body = {
      paymentAmount: props.invoice.totalAmount,
      paymentMethod: method
    }
    
    const res = await fetch(
      import.meta.env.VITE_API_URL + `/Invoices/${props.invoice.invoiceID}/pay`,
      {
        method: "POST",
        headers: { 
          "Content-Type": "application/json",
          "Authorization": `Bearer ${token}`
        },
        body: JSON.stringify(body)
      }
    )
    
    if (!res.ok) {
      const errorText = await res.text()
      throw new Error(`Ошибка оплаты: ${res.status} - ${errorText}`)
    }

    const blob = await res.blob()
    const url = window.URL.createObjectURL(blob)
    const link = document.createElement("a")
    link.href = url
    link.download = `receipt_${props.invoice.invoiceID}.pdf`
    document.body.appendChild(link)
    link.click()
    link.remove()
    window.URL.revokeObjectURL(url)

    alert("Оплата прошла успешно!")
    emit('payment-success')
  } catch (err: any) {
    alert(`Ошибка оплаты: ${err.message}`)
  }
}
</script>

<style scoped>
.step {
  max-width: 900px;
  margin: 0 auto;
  background: white;
  border-radius: var(--radius-lg);
  box-shadow: var(--shadow-sm);
  padding: 30px;
  margin-bottom: 25px;
}
button { font-weight: 600; border-radius: var(--radius-md); cursor: pointer; transition: var(--transition); }
.btn-next {
  background: linear-gradient(135deg, var(--color-navy), #3a4569);
  color: white;
  border: none;
  padding: 12px 20px;
  border-radius: var(--radius-md);
  font-weight: 600;
  cursor: pointer;
  transition: var(--transition);
}

.btn-next:hover {
  background: #2f3650;
  transform: translateY(-2px);
  box-shadow: var(--shadow-md);
}

.btn-prev {
  background: white;
  border: 2px solid var(--color-navy);
  color: var(--color-navy);
  padding: 12px 20px;
  border-radius: var(--radius-md);
  font-weight: 600;
  cursor: pointer;
  transition: var(--transition);
}

.btn-prev:hover {
  background: var(--color-navy);
  color: white;
  transform: translateY(-2px);
}

/* Заголовки */
.step h3 {
  font-size: 1.6rem;
  font-weight: 700;
  color: var(--color-navy);
  margin-bottom: 20px;
}

.step h4 {
  font-size: 1.2rem;
  font-weight: 600;
  color: var(--color-navy);
  margin: 20px 0 10px;
}

/* Текст */
.step p {
  margin: 6px 0;
  font-size: 0.95rem;
  color: var(--color-text);
}

.step p strong {
  color: var(--color-navy);
}

/* Счётчики */
.step ul {
  list-style: none;
  padding: 0;
  margin: 0;
}

.step li {
  background: #f8f9fa;
  border-radius: var(--radius-md);
  padding: 15px;
  margin-bottom: 12px;
  box-shadow: var(--shadow-sm);
}

.step li label {
  font-weight: 600;
  color: var(--color-navy);
  display: block;
  margin-bottom: 6px;
}

.step li input {
  width: 100%;
  padding: 10px;
  border-radius: var(--radius-md);
  border: 2px solid #e0e0e0;
  transition: var(--transition);
  margin-bottom: 6px;
}

.step li input:focus {
  border-color: var(--color-accent);
  box-shadow: 0 0 0 3px rgba(202, 62, 73, 0.15);
}

.step li small {
  font-size: 0.85rem;
  color: var(--color-muted);
}

/* Оплачен */
.paid,
.already-paid {
  text-align: center;
  padding: 20px;
  background: linear-gradient(135deg, #e6f4ea, #c8e6c9);
  border-radius: var(--radius-md);
  margin: 20px 0;
  color: #2f5d3a;
  font-weight: 600;
  box-shadow: var(--shadow-sm);
}

.already-paid .paid-icon {
  font-size: 40px;
  margin-bottom: 10px;
  display: inline-block;
  background: #2f5d3a;
  color: white;
  border-radius: 50%;
  width: 60px;
  height: 60px;
  line-height: 60px;
}

/* Кнопки */
.actions {
  display: flex;
  justify-content: flex-end;
  gap: 12px;
  margin-top: 20px;
}
.btn-pay { background: linear-gradient(135deg, var(--color-accent), #b8323d); color: white; border: none; padding: 14px 24px; font-size: 1.1rem; } .btn-pay:hover:enabled { background: #a12a34; transform: translateY(-2px); box-shadow: var(--shadow-md); } .btn-pay:disabled { background: #ccc; cursor: not-allowed; } /* Кнопка "Назад к счетам" */ .btn-back { display: inline-block; margin-bottom: 20px; padding: 10px 16px; border-radius: var(--radius-md); border: 2px solid var(--color-navy); background: white; color: var(--color-navy); } .btn-back:hover { background: var(--color-navy); color: white; transform: translateY(-2px); }
.actions button {
  padding: 12px 20px;
  border-radius: var(--radius-md);
  font-weight: 600;
  cursor: pointer;
  transition: var(--transition);
}

/* "Продолжить" */
.actions button:first-child {
  background: linear-gradient(135deg, var(--color-navy), #3a4569);
  color: white;
  border: none;
}

.actions button:first-child:hover {
  background: #2f3650;
  transform: translateY(-2px);
  box-shadow: var(--shadow-md);
}

/* "Назад" */
.actions button:last-child {
  background: white;
  border: 2px solid var(--color-navy);
  color: var(--color-navy);
}

.actions button:last-child:hover {
  background: var(--color-navy);
  color: white;
  transform: translateY(-2px);
}

</style>