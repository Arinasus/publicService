<template>
  <div class="step">
    <h3>Данные плательщика</h3>
    <p>ФИО: {{ payer.name }}</p>
    <p>Адрес: {{ payer.address }}</p>
    <p>Дата выставления: {{ new Date(invoice.issueDate).toLocaleDateString('ru-RU') }}</p>
    <p>Период: {{ invoice.period }}</p>
    <p v-if="invoice.isPaid" class="paid">Счёт уже оплачен</p>

    <h4>Счётчики</h4>
    <ul>
      <li v-for="m in meters" :key="m.serviceID">
        <label>{{ m.serviceName }}</label>
        <p>Предыдущее: {{ m.previousValue ?? 0 }}</p>
        <input 
          type="number" 
          v-model.number="m.currentValue" 
          :min="0"
          placeholder="Введите текущее значение" 
          class="meter-input"
        />
        <small>{{ m.unit }} · {{ m.price }} BYN</small>
      </li>
    </ul>

    <div class="actions">
      <button class="btn-next" @click="$emit('next')">Продолжить</button>
      <button class="btn-prev" @click="$emit('prev')">Назад</button>
    </div>
  </div>
</template>


<script setup lang="ts">
defineProps<{
  payer: { name: string; address: string }
  invoice: { issueDate: string; period: string; totalAmount: number; isPaid: boolean }
  meters: { serviceID: number; serviceName: string; previousValue: number; currentValue: number; unit: string; price: number }[]
}>()

</script>


<style scoped>
.paid {
  color: #2f5d3a;
  font-weight: 600;
  background: #e6f4ea;
  padding: 8px 12px;
  border-radius: var(--radius-md);
  display: inline-block;
  margin-top: 10px;
}

/* Счётчики */
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

.meter-input {
  width: 100%;
  padding: 10px;
  border-radius: var(--radius-md);
  border: 2px solid #e0e0e0;
  transition: var(--transition);
  margin-bottom: 6px;
}

.meter-input:focus {
  border-color: var(--color-accent);
  box-shadow: 0 0 0 3px rgba(202, 62, 73, 0.15);
}

/* Кнопки */
.actions {
  display: flex;
  justify-content: flex-end;
  gap: 12px;
  margin-top: 20px;
}

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

</style>
