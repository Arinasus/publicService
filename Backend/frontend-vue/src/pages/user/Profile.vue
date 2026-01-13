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
async function makePrimary(addressID: number) {
  try {
    const res = await apiFetch(`/UserAddresses/${addressID}/make-primary`, { method: "POST" })
    if (!res.ok) throw new Error(`Ошибка: ${res.status}`)
    // обновляем профиль
    await loadProfile()
  } catch (err: any) {
    alert(`Ошибка смены основного адреса: ${err.message}`)
  }
}




onMounted(async () => {
  await loadProfile()
  await loadCards()
})
</script>

<template>
  <div v-if="me" class="profile-wrapper fade-in">

    <!-- Заголовок -->
    <div class="section-header">
      <h1 class="title">Личный кабинет</h1>
      <p class="subtitle">Управление профилем, адресами и платёжными картами</p>
    </div>

    <!-- Блок профиля -->
    <div class="card profile-card shadow-lg">
      <h2 class="card-title">Профиль</h2>

      <div class="profile-info">
        <div class="info-row">
          <span class="label">Имя:</span>
          <span class="value">{{ me.firstName }} {{ me.lastName }}</span>
        </div>

        <div class="info-row">
          <span class="label">Email:</span>
          <span class="value">{{ me.email }}</span>
        </div>
      </div>
    </div>

    <!-- Адреса -->
    <div class="card shadow-lg">
      <h2 class="card-title">Адреса</h2>

      <ul class="list">
        <li v-for="addr in me.addresses" :key="addr.addressID" class="list-item">
          <div>
            <strong>{{ addr.addressLine }}</strong>
            <span v-if="addr.isPrimary" class="badge-primary">Основной</span>
          </div>

          <button 
            v-if="!addr.isPrimary"
            @click="makePrimary(addr.addressID)"
            class="btn-outline-auth btn-sm"
          >
            Сделать основным
          </button>
        </li>
      </ul>

      <!-- Добавить адрес -->
      <form @submit.prevent="addAddress" class="form-grid">
        <h3 class="form-title">Добавить адрес</h3>

        <input v-model="newAddress.street" class="form-control" placeholder="Улица" required>
        <input v-model="newAddress.city" class="form-control" placeholder="Город" required>
        <input v-model="newAddress.house" class="form-control" placeholder="Дом" required>
        <input v-model="newAddress.apartment" class="form-control" placeholder="Квартира">
        <input v-model="newAddress.postalCode" class="form-control" placeholder="Почтовый индекс">

        <button type="submit" class="btn-primary-auth mt-2">Добавить адрес</button>
      </form>
    </div>

    <!-- Карты -->
    <div class="card shadow-lg">
      <h2 class="card-title">Платёжные карты</h2>

      <ul class="list">
        <li v-for="card in cards" :key="card.id" class="list-item">
          <div>
            <strong>{{ card.number }}</strong>
            <div class="muted">{{ card.holder }} — {{ card.expiry }}</div>
          </div>

          <button @click="deleteCard(card.id)" class="btn-outline-auth btn-sm danger">
            Удалить
          </button>
        </li>
      </ul>

      <!-- Добавить карту -->
      <form @submit.prevent="saveCard" class="form-grid">
        <h3 class="form-title">Добавить карту</h3>

        <input v-model="newCard.number" class="form-control" placeholder="Номер карты" required>
        <input v-model="newCard.holder" class="form-control" placeholder="Владелец" required>
        <input v-model="newCard.expiry" class="form-control" placeholder="MM/YY" required>

        <button type="submit" class="btn-primary-auth mt-2">Добавить карту</button>
      </form>
    </div>

  </div>
</template>


<style scoped>
  .profile-wrapper {
  max-width: 900px;
  margin: 0 auto;
  padding: 30px 20px;
}

.section-header {
  text-align: center;
  margin-bottom: 40px;
}

.title {
  font-size: 2rem;
  font-weight: 700;
  color: var(--color-navy);
}

.subtitle {
  color: var(--color-muted);
  margin-top: 5px;
}

.card {
  background: white;
  border-radius: var(--radius-lg);
  padding: 25px 30px;
  margin-bottom: 35px;
}

.card-title {
  font-size: 1.4rem;
  font-weight: 600;
  margin-bottom: 20px;
  color: var(--color-navy);
}

.profile-info .info-row {
  display: flex;
  justify-content: space-between;
  padding: 8px 0;
}

.label {
  font-weight: 600;
  color: var(--color-muted);
}

.value {
  font-weight: 500;
}

.list {
  list-style: none;
  padding: 0;
  margin: 0 0 20px;
}

.list-item {
  display: flex;
  justify-content: space-between;
  padding: 12px 0;
  border-bottom: 1px solid #eee;
}

.badge-primary {
  background: var(--color-accent);
  color: white;
  padding: 3px 8px;
  border-radius: var(--radius-sm);
  font-size: 0.75rem;
  margin-left: 10px;
}

.form-grid {
  display: grid;
  gap: 12px;
  margin-top: 15px;
}

.form-title {
  font-size: 1.1rem;
  font-weight: 600;
  margin-bottom: 5px;
}

.form-control {
  padding: 12px;
  border-radius: var(--radius-md);
  border: 2px solid #e0e0e0;
  transition: var(--transition);
}

.form-control:focus {
  border-color: var(--color-accent);
  box-shadow: 0 0 0 3px rgba(202, 62, 73, 0.15);
}

.btn-primary-auth {
  width: 100%;
  padding: 12px;
  border-radius: var(--radius-md);
  background: linear-gradient(135deg, #2f3650, #3a4569);
  border: none;
  color: white;
  font-weight: 600;
  transition: var(--transition);
}

.btn-primary-auth:hover {
  transform: translateY(-2px);
  box-shadow: var(--shadow-md);
}

.btn-outline-auth {
  padding: 8px 14px;
  border-radius: var(--radius-md);
  border: 2px solid var(--color-navy);
  background: white;
  color: var(--color-navy);
  font-weight: 600;
  transition: var(--transition);
}

.btn-outline-auth:hover {
  background: var(--color-navy);
  color: white;
}

.btn-outline-auth.danger {
  border-color: var(--color-accent);
  color: var(--color-accent);
}

.btn-outline-auth.danger:hover {
  background: var(--color-accent);
  color: white;
}

</style>

<!-- docker compose exec postgres psql -U util_user -d utilities
-->