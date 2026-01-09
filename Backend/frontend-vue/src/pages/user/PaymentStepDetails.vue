<template>
  <div class="step">
    <h3>Данные плательщика</h3>
    <p>ФИО: {{ payer.name }}</p>
    <p>Адрес: {{ payer.address }}</p>
    <p>Дата выставления: {{ new Date(invoice.issueDate).toLocaleDateString('ru-RU') }}</p>
    <p>Период: {{ invoice.period }}</p>
    <p v-if="invoice.isPaid" class="paid">✅ Счёт уже оплачен</p>

    <h4>Счётчики</h4>
    <ul>
<li v-for="m in meters" :key="m.name">
  <label>{{ m.name }}</label>
  <p>Предыдущее: {{ m.previous }}</p>
  <input type="number" v-model.number="m.current" placeholder="Введите текущее значение" />
</li>

    </ul>

    <div class="actions">
      <button @click="$emit('next')">Продолжить</button>
      <button @click="$emit('prev')">Назад</button>
    </div>
  </div>
</template>

<script setup lang="ts">
defineProps<{
  payer: { name: string; address: string }
  invoice: { issueDate: string; period: string; totalAmount: number; isPaid: boolean }
  meters: { name: string; previous: number; current: number; unit: string; price: number }[]
}>()
</script>


<style scoped>
.paid {
  color: green;
  font-weight: bold;
}
</style>
