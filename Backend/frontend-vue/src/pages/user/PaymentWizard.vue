<template>
  <div class="payment-wizard">
    <button @click="$emit('back')">← Назад к счетам</button>

    <component
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

const step = ref(1)
const paymentMethod = ref("Card")

// данные из API
const provider = ref<any>(null)
const payer = ref<any>(null)
const meters = ref<any[]>([])

onMounted(async () => {
  try {
    const res = await fetch(import.meta.env.VITE_API_URL + `/InvoiceDetails/${props.invoice.invoiceID}`)
    if (!res.ok) throw new Error(`Ошибка загрузки: ${res.status}`)
    const data = await res.json()

    provider.value = data.provider
    payer.value = data.payer
    meters.value = data.meters
  } catch (err: any) {
    alert(`Ошибка загрузки данных: ${err.message}`)
  }
})

const nextStep = () => { if (step.value < 3) step.value++ }
const prevStep = () => { if (step.value > 1) step.value-- }

const currentStepComponent = computed(() => {
  switch (step.value) {
    case 1: return PaymentStepProvider
    case 2: return PaymentStepDetails
    case 3: return PaymentStepConfirm
  }
})

// обновление метода оплаты из дочернего компонента
function updatePaymentMethod(val: string) {
  paymentMethod.value = val
}

// оплата
async function payInvoice(method: string) {
  try {
    const body = {
      paymentAmount: props.invoice.totalAmount,
      paymentMethod: method
    }
    const res = await fetch(
      import.meta.env.VITE_API_URL + `/Invoices/${props.invoice.invoiceID}/pay`,
      {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(body)
      }
    )
    if (!res.ok) throw new Error(`Ошибка оплаты: ${res.status}`)

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
  } catch (err: any) {
    alert(`Ошибка оплаты: ${err.message}`)
  }
}
</script>
