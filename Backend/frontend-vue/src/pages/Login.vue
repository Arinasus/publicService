<script setup lang="ts">
import {ref} from 'vue'
import {useRouter, useRoute} from 'vue-router'

const router = useRouter()
const route = useRoute()
const email = ref('')
const password = ref('')
const role = ref<'admin'|'user'>('user')

async function login() {
  const res = await fetch(import.meta.env.VITE_API_URL + '/Auth/login', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ email: email.value, password: password.value })
  })

  if (!res.ok) {
    alert('Ошибка входа')
    return
  }

  const data = await res.json()
  localStorage.setItem('auth', JSON.stringify(data))

  const redirect = (route.query.redirect as string) ?? (data.role === 'admin' ? '/admin' : '/')
  router.push(redirect)
}

</script>
<template>
  <div>
    <h2>Вход</h2>
    <form @submit.prevent="login">
      <div>
        <label>Email</label>
        <input v-model="email" type="email" />
      </div>
      <div>
        <label>Пароль</label>
        <input v-model="password" type="password" />
      </div>
      <div>
        <label>Роль</label>
        <select v-model="role">
          <option value="user">Пользователь</option>
          <option value="admin">Администратор</option>
        </select>
      </div>
      <button type="submit">Войти</button>
    </form>
  </div>
</template>