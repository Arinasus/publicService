<template>
  <div class="step">
    <h3>Подтверждение</h3>

    <li v-for="m in meters" :key="m.name">
      <label>{{ m.name }}</label>
      <p>Предыдущее: {{ m.previous }}</p>
      <p>Текущее: {{ m.current }}</p>
      <p>Расход: {{ (m.current ?? 0) - (m.previous ?? 0) }} {{ m.unit }}</p>
      <p>Стоимость: {{ (((m.current ?? 0) - (m.previous ?? 0)) * (m.price ?? 0)).toFixed(2) }} BYN</p>
    </li>

    <p>Итого к оплате: {{ totalAmount.toFixed(2) }} BYN</p>

    <p v-if="invoice.isPaid" class="paid">✅ Счёт уже оплачен</p>

    <h4>Выберите карту для оплаты</h4>
    <select v-model="selectedCard">
<option v-for="card in availableCards" :key="card.cardID" :value="card.cardID">
  {{ card.cardType }} {{ card.maskedNumber }} (до {{ card.expiry }})
</option>

    </select>

    <div class="actions">
      <button @click="$emit('pay', selectedCard)" :disabled="invoice.isPaid || !selectedCard">Оплатить</button>
      <button @click="$emit('prev')">Назад</button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'

// тип для карты
interface Card {
  cardID: number
  maskedNumber: string
  cardType: string
  expiry: string
}

// список карт
const availableCards = ref<Card[]>([])

// ⚠️ token нужно получить из стора/локального хранилища или передать как проп
const token = localStorage.getItem('token') ?? ''

onMounted(async () => {
  const res = await fetch('http://localhost:5000/api/Cards/me', {
  headers: { Authorization: 'Bearer ' + token }
})

  if (res.ok) {
    availableCards.value = await res.json()
  } else {
    console.error('Ошибка загрузки карт', await res.text())
  }
})

const props = defineProps<{
  meters: { name: string; previous: number; current: number; unit: string; price: number }[]
  invoice: { totalAmount: number; isPaid: boolean }
  payer: { name: string; address: string }
}>()

const totalAmount = computed(() =>
  props.meters.reduce((sum, m) => sum + (m.current - m.previous) * m.price, 0)
)

// выбранная карта
const selectedCard = ref<number | null>(null)

const emit = defineEmits<{
  (e: 'pay', cardID: number | null): void
  (e: 'prev'): void
}>()
</script>

<style scoped>
.paid {
  color: green;
  font-weight: bold;
}
</style>
