<script setup lang="ts">
import { ref, onMounted } from 'vue'
type Invoice = { id: number; amount: number; status: 'Оплачен' | 'Не оплачен' }
const invoices = ref<Invoice[]>([])
onMounted(async () => {
  // TODO: GET /api/user/invoices
  invoices.value = [
    { id: 101, amount: 24.87, status: 'Оплачен' },
    { id: 102, amount: 18.34, status: 'Не оплачен' },
  ]
})
</script>

<template>
  <div>
    <h2>Мои счета</h2>
    <ul>
      <li v-for="inv in invoices" :key="inv.id">
        Счёт #{{ inv.id }} — {{ inv.amount }} BYN — {{ inv.status }}
      </li>
    </ul>
  </div>
</template>
