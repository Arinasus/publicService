
<template>
  <link href="https://fonts.googleapis.com/css2?family=Material+Symbols+Outlined" rel="stylesheet" />

  <div class="step">
    <h3>Подтверждение оплаты</h3>
    
    <div class="summary-card">
      <div class="summary-header">
        <h4>Детали счёта</h4>
        <span class="invoice-number">№ {{ invoice.invoiceID }}</span>
      </div>
      
      <div class="summary-body">
        <div class="payer-info">
          <p><strong>Плательщик:</strong> {{ payer.name }}</p>
          <p><strong>Адрес:</strong> {{ payer.address }}</p>
          <p><strong>Период:</strong> {{ invoice.period }}</p>
        </div>
        
        <div v-if="meters.length > 0" class="consumption-details">
          <h5>Расчёт по услугам:</h5>
          <table class="consumption-table">
            <thead>
              <tr>
                <th>Услуга</th>
                <th>Предыдущее</th>
                <th>Текущее</th>
                <th>Расход</th>
                <th>Тариф</th>
                <th>Стоимость</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="meter in meters" :key="meter.serviceID">
                <td>{{ meter.serviceName }}</td>
                <td>{{ Number(meter.previousValue ?? 0).toFixed(2) }}</td>
                <td>{{ Number(meter.currentValue ?? 0).toFixed(2) }}</td>
                <td>{{ consumption(meter).toFixed(2) }} {{ meter.unit }}</td>
                <td>{{ meter.price }} BYN</td>
                <td>{{ calculateAmount(meter) }} BYN</td>
              </tr>
            </tbody>
          </table>
        </div>
        
        <div class="total-section">
          <div class="total-row">
            <span>Итого к оплате:</span>
            <span class="total-amount">{{ totalAmount.toFixed(2) }} BYN</span>
          </div>
        </div>
      </div>
    </div>
    
    <div v-if="invoice.isPaid" class="already-paid">
<div class="paid-icon">
  <span class="material-symbols-outlined">task_alt</span>
</div>



      <h4>Счёт уже оплачен</h4>
      <p>Дата оплаты: {{ formatDate(invoice.paymentDate) }}</p>
    </div>
    
    <div v-else class="payment-method">
      <h4>Выберите способ оплаты</h4>
      
      <div class="payment-options">
        <label 
          v-for="method in paymentMethods" 
          :key="method.value"
          class="payment-option"
          :class="{ selected: paymentMethod === method.value }"
        >
          <input 
            type="radio" 
            :value="method.value" 
            v-model="paymentMethod"
            @change="$emit('update:paymentMethod', paymentMethod)"
          />
          <div class="option-content">
            <span class="option-icon">{{ method.icon }}</span>
            <div>
              <span class="option-name">{{ method.name }}</span>
              <span class="option-description">{{ method.description }}</span>
            </div>
          </div>
        </label>
      </div>
      
      <!-- Выбор карты -->
      <div v-if="paymentMethod === 'Card' && availableCards.length > 0" class="card-selection">
        <h5>Выберите карту:</h5>
        <select v-model="selectedCard" class="card-select">
          <option v-for="card in availableCards" :key="card.cardID" :value="card.cardID">
  {{ formatCardType(card.cardType) }} •••• {{ (card.maskedNumber?.slice(-4) || '') }} 
  (до {{ card.expiry || '—' }})
</option>

        </select>
      </div>
      
<div v-else-if="paymentMethod === 'Card'" class="no-cards">
  <p>У вас нет сохранённых карт.</p>
</div>

      
      <div v-if="paymentMethod === 'BankTransfer'" class="bank-info">
        <p><strong>Реквизиты для перевода:</strong></p>
        <p>Получатель: {{ getProviderName() }}</p>
        <p>IBAN: {{ getProviderIBAN() }}</p>
        <p>УНП: {{ getProviderUNP() }}</p>
        <p><em>После перевода сохраните квитанцию</em></p>
      </div>
    </div>
    
    <div class="confirmation-actions">
  <button 
    class="btn-pay" 
    @click="handlePayment" 
    :disabled="invoice.isPaid || (paymentMethod === 'Card' && !selectedCard)"
  >
    {{ payButtonText }}
  </button>
  <button class="btn-prev" @click="$emit('prev')">Назад</button>
</div>

    
    <div v-if="error" class="error-message">
      {{ error }}
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'

// Интерфейсы
interface Card {
  cardID: number
  maskedNumber?: string
  cardType: string
  expiry: string
}

interface ServiceMeter {
  serviceID: number
  serviceName: string
  previousValue: number
  currentValue: number
  unit: string
  price: number
}

interface Payer {
  name: string
  address: string
}

interface Invoice {
  invoiceID: number
  totalAmount: number
  isPaid: boolean
  paymentDate?: string
  period: string
}

interface Provider {
  providerName: string
  iban: string
  unp: string
}

interface PaymentMethod {
  value: string
  name: string
  icon: string
  description: string
}

// Пропсы
const props = defineProps<{
  meters: ServiceMeter[]
  invoice: Invoice
  payer: Payer
  provider?: Provider
}>()

const emit = defineEmits<{
  (e: 'pay', method: string, cardId?: number): void
  (e: 'prev'): void
  (e: 'update:paymentMethod', method: string): void
}>()

// Данные
const availableCards = ref<Card[]>([])
const selectedCard = ref<number | null>(null)
const paymentMethod = ref<string>('Card')
const error = ref<string>('')

// Методы оплаты
const paymentMethods: PaymentMethod[] = [
  { value: 'Card', name: 'Банковская карта', icon: '💳', description: 'Мгновенная оплата' },
  { value: 'BankTransfer', name: 'Банковский перевод', icon: '🏦', description: 'Ручной перевод' }
]

// Загрузка карт
onMounted(async () => {
  await loadCards()
})

async function loadCards() {
  try {
    const rawAuth = localStorage.getItem('auth')
    if (!rawAuth) {
      console.warn('Пользователь не авторизован')
      return
    }
    
    const auth = JSON.parse(rawAuth)
    const token = auth.token
    
    if (!token) {
      console.warn('Токен не найден')
      return
    }
    
    const res = await fetch('http://localhost:5000/api/Cards/me', {
      headers: { 
        'Authorization': 'Bearer ' + token 
      }
    })
    
    if (res.ok) {
      const cards = await res.json()
      // Убеждаемся, что это массив
      availableCards.value = Array.isArray(cards) ? cards : []
      
      // Безопасное присваивание
      if (availableCards.value.length > 0) {
        const firstCard = availableCards.value[0]
        if (firstCard && firstCard.cardID) {
          selectedCard.value = firstCard.cardID
        }
      }
    } else {
      const errorText = await res.text()
      console.error('Ошибка загрузки карт:', res.status, errorText)
    }
  } catch (err) {
    console.error('Ошибка загрузки карт:', err)
  }
}

// Вспомогательные функции для безопасного доступа к provider
function getProviderName(): string {
  return props.provider?.providerName || 'Поставщик'
}

function getProviderIBAN(): string {
  return props.provider?.iban || 'Не указан'
}

function getProviderUNP(): string {
  return props.provider?.unp || 'Не указан'
}

// Расчёты
function consumption(meter: ServiceMeter): number {
  return Number(meter.currentValue ?? 0) - Number(meter.previousValue ?? 0)
}


function calculateAmount(meter: ServiceMeter): string { 
  return (consumption(meter) * Number(meter.price ?? 0)).toFixed(2) }

const totalAmount = computed(() => {
  return props.meters.reduce((sum, meter) => {
    return sum + (consumption(meter) * meter.price)
  }, 0)
})

// Форматирование
function formatDate(dateString?: string): string {
  if (!dateString) return 'неизвестно'
  return new Date(dateString).toLocaleDateString('ru-RU')
}

function formatCardType(cardType: string): string {
  const types: Record<string, string> = {
    'Visa': 'Visa',
    'MasterCard': 'MasterCard',
    'Mir': 'Мир',
    'Belcard': 'Белкарт'
  }
  return types[cardType] || cardType
}

// Текст кнопки оплаты
const payButtonText = computed(() => {
  if (props.invoice.isPaid) return 'Оплачено'
  if (paymentMethod.value === 'Card') return `Оплатить ${totalAmount.value.toFixed(2)} BYN`
  return 'Подтвердить платёж'
})

// Обработка оплаты
async function handlePayment() {
  if (props.invoice.isPaid) {
    alert('Этот счёт уже оплачен')
    return
  }

  if (paymentMethod.value === 'Card' && !selectedCard.value) {
    error.value = 'Выберите карту для оплаты'
    return
  }

  try {
    const rawAuth = localStorage.getItem('auth')
    if (!rawAuth) {
      error.value = 'Нет авторизации'
      return
    }
    const auth = JSON.parse(rawAuth)
    const token = auth.token

    const paymentDto = {
      paymentAmount: totalAmount.value,
      paymentMethod: paymentMethod.value,
      cardID: selectedCard.value,
      meters: props.meters.map(m => ({
        serviceID: m.serviceID,
        currentValue: m.currentValue
      }))
    }

    const res = await fetch(`http://localhost:5000/api/Invoices/${props.invoice.invoiceID}/pay`, {
      method: 'POST',
      headers: {
        'Authorization': 'Bearer ' + token,
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(paymentDto)
    })

    if (!res.ok) {
      const text = await res.text()
      throw new Error(text)
    }

   try {
  // POST /pay
  const result = await res.json()
  alert(`Оплата прошла успешно. Номер операции: ${result.paymentID}`)

  // GET /receipt
  await downloadReceipt(props.invoice.invoiceID, token)
} catch (err: any) {
  console.error('Ошибка при скачивании чека:', err)
  error.value = 'Не удалось скачать чек: ' + err.message
}


  } catch (err: any) {
    console.error('Ошибка оплаты:', err)
    error.value = 'Ошибка оплаты: ' + err.message
  }
}
async function downloadReceipt(invoiceId: number, token: string) {
  const res = await fetch(`http://localhost:5000/api/Invoices/${invoiceId}/receipt`, {
    headers: { 'Authorization': 'Bearer ' + token }
  })
  if (!res.ok) {
    const text = await res.text()
    throw new Error(text)
  }
  const blob = await res.blob()
  const url = window.URL.createObjectURL(blob)
  const a = document.createElement('a')
  a.href = url
  a.download = `receipt_${invoiceId}.pdf`
  a.click()
  window.URL.revokeObjectURL(url)
}


</script>

<style scoped>
.paid-icon {
  font-size: 48px;
  color: #2f5d3a;
  margin-bottom: 15px;
}

.already-paid {
  text-align: center;
  padding: 30px;
  background: linear-gradient(135deg, #e6f4ea, #c8e6c9);
  border-radius: var(--radius-lg);
  margin: 30px 0;
  color: #2f5d3a;
  box-shadow: var(--shadow-sm);
}

.already-paid h4 {
  margin: 10px 0;
  font-weight: 700;
  font-size: 1.2rem;
}

/* Карточка */
.step {
  max-width: 900px;
  margin: 0 auto;
  background: white;
  border-radius: var(--radius-lg);
  box-shadow: var(--shadow-sm);
  padding: 30px;
  margin-bottom: 25px;
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

/* Таблица расчётов */
.consumption-table th {
  background: #f8f9fa;
  padding: 12px;
  text-align: left;
  font-weight: 600;
  color: #333;
  border-bottom: 2px solid #e0e0e0;
}

.consumption-table td {
  padding: 12px;
  border-bottom: 1px solid #eee;
}

.consumption-table tr:hover {
  background: #f9f9f9;
}

/* Итого */
.total-section {
  text-align: right;
  padding-top: 20px;
  border-top: 2px solid var(--color-navy);
}

.total-row {
  display: inline-flex;
  justify-content: space-between;
  align-items: center;
  min-width: 300px;
  font-size: 20px;
  font-weight: bold;
}

.total-amount {
  color: var(--color-accent);
  font-size: 24px;
}

/* Оплачен */
.already-paid {
  text-align: center;
  padding: 30px;
  background: linear-gradient(135deg, #e6f4ea, #c8e6c9);
  border-radius: var(--radius-lg);
  margin: 30px 0;
  color: #2f5d3a;
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

/* Методы оплаты */
.payment-options {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
  gap: 15px;
  margin: 20px 0;
}

.payment-option {
  border: 2px solid #e0e0e0;
  border-radius: var(--radius-md);
  padding: 15px;
  cursor: pointer;
  transition: all 0.3s ease;
}

.payment-option:hover {
  border-color: var(--color-navy);
  background: #f5f9ff;
}

.payment-option.selected {
  border-color: var(--color-navy);
  background: #e3f2fd;
}

.payment-option input[type="radio"] {
  display: none;
}

.option-content {
  display: flex;
  align-items: center;
  gap: 15px;
}

.option-icon {
  font-size: 24px;
}

.option-name {
  font-weight: 600;
  margin-bottom: 4px;
  color: var(--color-navy);
}

.option-description {
  font-size: 14px;
  color: #666;
}

/* Выбор карты */
.card-selection {
  margin: 25px 0;
  padding: 20px;
  background: #f8f9fa;
  border-radius: var(--radius-md);
}

.card-select {
  width: 100%;
  padding: 12px;
  border: 1px solid #ddd;
  border-radius: var(--radius-md);
  font-size: 16px;
  background: white;
  cursor: pointer;
}

.no-cards {
  text-align: center;
  padding: 30px;
  background: #f8f9fa;
  border-radius: var(--radius-md);
  margin: 20px 0;
  color: var(--color-muted);
}

/* Банковский перевод */
.bank-info {
  padding: 20px;
  background: #f0f4ff;
  border-radius: var(--radius-md);
  margin: 20px 0;
  color: var(--color-navy);
}

/* Кнопки */
.confirmation-actions {
  display: flex;
  gap: 20px;
  margin-top: 40px;
}

.btn-pay {
  flex: 2;
  background: linear-gradient(135deg, var(--color-navy), #3a4569);
  color: white;
  font-size: 18px;
  font-weight: bold;
  padding: 18px;
  border: none;
  border-radius: var(--radius-md);
  cursor: pointer;
  transition: var(--transition);
}

.btn-pay:hover:enabled {
  background: #2f3650;
  transform: translateY(-2px);
  box-shadow: var(--shadow-md);
}

.btn-pay:disabled {
  background: #ccc;
  cursor: not-allowed;
}

.btn-prev {
  flex: 1;
  background: white;
  border: 2px solid var(--color-navy);
  color: var(--color-navy);
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

/* Ошибки */
.error-message {
  color: var(--color-accent);
  background: #ffebee;
  padding: 15px;
  border-radius: var(--radius-md);
  margin-top: 20px;
  text-align: center;
}

</style>