<template>
  <div class="step">
    <h3>Реквизиты получателя</h3>
    <p v-if="provider && provider.providerName">Поставщик: {{ provider.providerName }}</p>
    <p v-else>Поставщик не указан</p>
    <p>Контактное лицо: {{ provider.contactPerson }}</p>
    <p>Телефон: {{ provider.phoneNumber }}</p>
    <p>Email: {{ provider.email }}</p>
    <p>IBAN: {{ provider.iban }}</p>
    <p>BIC: {{ provider.bic }}</p>
    <p>УНП: {{ provider.unp }}</p>
    <p>Номер плательщика (ContractID): {{ contractID }}</p>

    <div class="actions">
      <button  class="btn-next" @click="$emit('next')">Продолжить</button>
      <button class="btn-prev" @click="$emit('prev')">Назад</button>
    </div>
  </div>
</template>

<script setup lang="ts">
defineProps<{
  provider: {
    providerID: number
    providerName: string
    contactPerson: string
    phoneNumber: string
    email: string
    iban: string
    bic: string
    unp: string
  }
  contractID: number
}>()
</script>
<style scoped>
  .step p {
  margin: 6px 0;
  font-size: 0.95rem;
  color: var(--color-text);
}

.step p strong {
  color: var(--color-navy);
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