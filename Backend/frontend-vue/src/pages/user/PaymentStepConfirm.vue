<template>
  <div class="step">
    <h3>Подтверждение</h3>
    <p>Сумма: {{ invoice.totalAmount.toFixed(2) }} BYN</p>
    <p v-if="invoice.isPaid" class="paid">✅ Счёт уже оплачен</p>

    <label>
      Способ оплаты:
      <select v-model="localMethod">
        <option value="Card">Карта</option>
        <option value="Cash">Наличные</option>
      </select>
    </label>

    <div class="actions">
      <button @click="$emit('pay', localMethod)" :disabled="invoice.isPaid">Оплатить</button>
      <button @click="$emit('prev')">Назад</button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue'

// входные данные
const props = defineProps<{
  invoice: { totalAmount: number; isPaid: boolean }
  paymentMethod: string
}>()

// события для связи с родителем
const emit = defineEmits<{
  (e: 'update:paymentMethod', value: string): void
  (e: 'pay', value: string): void
  (e: 'prev'): void
}>()

// локальное состояние
const localMethod = ref(props.paymentMethod)

// следим за изменением и пробрасываем наверх
watch(localMethod, (val) => {
  emit('update:paymentMethod', val)
})
</script>

<style scoped>
.paid {
  color: green;
  font-weight: bold;
}
</style>
