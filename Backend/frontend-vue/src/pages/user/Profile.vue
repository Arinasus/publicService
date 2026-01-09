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
const newAddress = ref({
  street: '',
  city: '',
  house: '',
  apartment: '',
  postalCode: ''
})

// загрузка профиля
import { apiFetch } from '../../services/apiFetch'

async function loadProfile() {
  try {
    const res = await apiFetch('/Users/me')
    if (!res.ok) throw new Error(`Ошибка загрузки профиля: ${res.status}`)
    me.value = await res.json()
  } catch (err: any) {
    alert(`Ошибка загрузки профиля: ${err.message}`)
  }
}


// загрузка карт
async function loadCards() {
try { 
  const res = await apiFetch('/Cards/me')
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

// добавить карту
async function saveCard() {
  if (!newCard.value.number || !newCard.value.holder || !newCard.value.expiry) {
    alert('Заполните все поля')
    return
  }
   else {
    try {
      const body = {
        cardNumber: newCard.value.number,
        expiryDate: newCard.value.expiry,
        cardHolderName: newCard.value.holder
      }
      const res = await apiFetch('/Cards', {
      method: "POST",
      headers: { "Content-Type": "application/json" },
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

// удалить карту
async function deleteCard(id: number) {
  try {
    const res = await apiFetch(`/Cards/${id}`, { method: "DELETE" })
    if (!res.ok) throw new Error(`Ошибка удаления: ${res.status}`)
    await loadCards()
  } catch (err: any) {
    alert(`Ошибка удаления карты: ${err.message}`)
  }
}
async function addAddress() {
  if (!newAddress.value.street || !newAddress.value.city || !newAddress.value.house) {
    alert('Заполните обязательные поля: улица, город, дом')
    return
  }

  try {
    //создаём адрес
    const resAddr = await apiFetch('/Addresses', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({
        street: newAddress.value.street,
        city: newAddress.value.city,
        house: newAddress.value.house,
        apartment: newAddress.value.apartment,
        postalCode: newAddress.value.postalCode
      })
    })

    if (!resAddr.ok) throw new Error(`Ошибка добавления адреса: ${resAddr.status}`)
    const createdAddr = await resAddr.json()

    //создаём связь с пользователем
    const resLink = await apiFetch('/UserAddresses', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({
        userID: me.value?.userID,
        addressID: createdAddr.addressID,
        isPrimary: false
      })
    })

    if (!resLink.ok) throw new Error(`Ошибка привязки адреса: ${resLink.status}`)

    //обновляем список адресов в профиле
    me.value?.addresses.push({
      addressID: createdAddr.addressID,
      addressLine: `${createdAddr.street}, ${createdAddr.house}, ${createdAddr.city}`,
      isPrimary: false
    })

    // очистка формы
    newAddress.value = { street: '', city: '', house: '', apartment: '', postalCode: '' }
  } catch (err: any) {
    alert(`Ошибка добавления адреса: ${err.message}`)
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
    <h3>Добавить адрес</h3>
<form @submit.prevent="addAddress">
  <input v-model="newAddress.street" placeholder="Улица" required />
  <input v-model="newAddress.city" placeholder="Город" required />
  <input v-model="newAddress.house" placeholder="Дом" required />
  <input v-model="newAddress.apartment" placeholder="Квартира" />
  <input v-model="newAddress.postalCode" placeholder="Почтовый индекс" />
  <button type="submit">Добавить</button>
</form>


    <h3>Мои карты</h3>
    <ul>
      <li v-for="card in cards" :key="card.id">
        {{ card.number }} ({{ card.holder }}, {{ card.expiry }})
        <button @click="deleteCard(card.id)">Удалить</button>
      </li>
    </ul>

    <h3>Добавить карту</h3>
    <form @submit.prevent="saveCard">
      <input v-model="newCard.number" placeholder="Номер карты" />
      <input v-model="newCard.holder" placeholder="Владелец" />
      <input v-model="newCard.expiry" placeholder="Срок действия (MM/YY)" />
      <button type="submit">Добавить</button>
    </form>
  </div>
</template>



<!-- docker compose exec postgres psql -U util_user -d utilities
-->