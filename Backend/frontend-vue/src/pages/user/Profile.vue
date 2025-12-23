<script setup lang="ts">
import { ref, onMounted } from 'vue'

interface Address {
  addressID: number
  addressLine: string
  isPrimary: boolean
}

interface UserProfile {
  userID: number
  firstName: string
  lastName: string
  email: string
  addresses: Address[]
}

const me = ref<UserProfile | null>(null)
const cards = ref<{ id: number; number: string; holder: string; expiry: string }[]>([])
const newCard = ref({ number: '', holder: '', expiry: '' })
const editingCardId = ref<number | null>(null)

// загрузка профиля
async function loadProfile() {
  try {
    const res = await fetch(import.meta.env.VITE_API_URL + "/Users/me", {
      headers: {
        "Authorization": `Bearer ${localStorage.getItem("token")}`
      }
    })
    if (!res.ok) throw new Error(`Ошибка загрузки профиля: ${res.status}`)
    me.value = await res.json()
  } catch (err: any) {
    alert(`Ошибка загрузки профиля: ${err.message}`)
  }
}

// загрузка карт
async function loadCards() {
  try {
    const res = await fetch(import.meta.env.VITE_API_URL + "/Cards/me", {
      headers: {
        "Authorization": `Bearer ${localStorage.getItem("token")}`
      }
    })
    if (!res.ok) throw new Error(`Ошибка загрузки карт: ${res.status}`)
    const data = await res.json()
    cards.value = data.map((c: any) => ({
      id: c.cardID,
      number: c.cardNumber,
      holder: c.cardHolderName,
      expiry: c.expiryDate
    }))
  } catch (err: any) {
    alert(`Ошибка загрузки карт: ${err.message}`)
  }
}

// добавить или обновить карту
async function saveCard() {
  if (!newCard.value.number || !newCard.value.holder || !newCard.value.expiry) {
    alert('Заполните все поля')
    return
  }

  if (editingCardId.value) {
    // обновление
    try {
      const body = {
        cardID: editingCardId.value,
        cardNumber: newCard.value.number,
        expiryDate: newCard.value.expiry,
        cardHolderName: newCard.value.holder
      }
      const res = await fetch(import.meta.env.VITE_API_URL + `/Cards/${editingCardId.value}`, {
        method: "PUT",
        headers: {
          "Content-Type": "application/json",
          "Authorization": `Bearer ${localStorage.getItem("token")}`
        },
        body: JSON.stringify(body)
      })
      if (!res.ok) throw new Error(`Ошибка обновления: ${res.status}`)
      await loadCards()
      editingCardId.value = null
      newCard.value = { number: '', holder: '', expiry: '' }
    } catch (err: any) {
      alert(`Ошибка обновления карты: ${err.message}`)
    }
  } else {
    // добавление
    try {
      const body = {
        cardNumber: newCard.value.number,
        expiryDate: newCard.value.expiry,
        cardHolderName: newCard.value.holder
      }
      const res = await fetch(import.meta.env.VITE_API_URL + "/Cards", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          "Authorization": `Bearer ${localStorage.getItem("token")}`
        },
        body: JSON.stringify(body)
      })
      if (!res.ok) throw new Error(`Ошибка добавления: ${res.status}`)
      await loadCards()
      newCard.value = { number: '', holder: '', expiry: '' }
    } catch (err: any) {
      alert(`Ошибка добавления карты: ${err.message}`)
    }
  }
}

// редактировать карту
function editCard(card: any) {
  newCard.value = { number: card.number, holder: card.holder, expiry: card.expiry }
  editingCardId.value = card.id
}

// удалить карту
async function deleteCard(id: number) {
  try {
    const res = await fetch(import.meta.env.VITE_API_URL + `/Cards/${id}`, {
      method: "DELETE",
      headers: {
        "Authorization": `Bearer ${localStorage.getItem("token")}`
      }
    })
    if (!res.ok) throw new Error(`Ошибка удаления: ${res.status}`)
    await loadCards()
  } catch (err: any) {
    alert(`Ошибка удаления карты: ${err.message}`)
  }
}

onMounted(async () => {
  await loadProfile()
  await loadCards()
})
</script>

<template>
  <div v-if="me">
    <h2>Профиль</h2>
    <p>{{ me.firstName }} {{ me.lastName }}</p>
    <p>{{ me.email }}</p>

    <h3>Адреса</h3>
    <ul>
      <li v-for="addr in me.addresses" :key="addr.addressID">
        {{ addr.addressLine }} <span v-if="addr.isPrimary">⭐ основной</span>
      </li>
    </ul>

    <h3>Мои карты</h3>
    <ul>
      <li v-for="card in cards" :key="card.id">
        {{ card.number }} ({{ card.holder }}, {{ card.expiry }})
        <button @click="editCard(card)">Изменить</button>
        <button @click="deleteCard(card.id)">Удалить</button>
      </li>
    </ul>

    <h3>{{ editingCardId ? 'Редактировать карту' : 'Добавить карту' }}</h3>
    <form @submit.prevent="saveCard">
      <input v-model="newCard.number" placeholder="Номер карты" />
      <input v-model="newCard.holder" placeholder="Владелец" />
      <input v-model="newCard.expiry" placeholder="Срок действия (MM/YY)" />
      <button type="submit">{{ editingCardId ? 'Сохранить' : 'Добавить' }}</button>
    </form>
  </div>
</template>



<!-- docker compose exec postgres psql -U util_user -d utilities
-->