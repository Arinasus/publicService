<template>
  <div class="step">
    <h3>Реквизиты получателя</h3>
    <p v-if="provider.providerName">Поставщик: {{ provider.providerName }}</p>
    <p v-else>Поставщик не указан</p>
    <p>Контактное лицо: {{ provider.contactPerson }}</p>
    <p>Телефон: {{ provider.phoneNumber }}</p>
    <p>Email: {{ provider.email }}</p>
    <p>IBAN: {{ provider.iban }}</p>
    <p>BIC: {{ provider.bic }}</p>
    <p>УНП: {{ provider.unp }}</p>
    <p>Номер плательщика (ContractID): {{ contractID }}</p>

    <div class="actions">
      <button @click="$emit('next')">Продолжить</button>
      <button @click="$emit('prev')">Назад</button>
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
