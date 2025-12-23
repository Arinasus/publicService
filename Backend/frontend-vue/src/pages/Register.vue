<template>
  <div class="container py-5">
    <div class="row justify-content-center">
      <div class="col-12 col-md-8 col-lg-6 col-xl-5">
        <div class="card login-card shadow-lg border-0 overflow-hidden">
          <div class="card-body p-4 p-md-5 position-relative z-1">
            <div class="text-center mb-5">
              <h1 class="h2 fw-bold mb-2 text-dark-emphasis">Регистрация</h1>
              <p class="text-muted">Создайте новый аккаунт</p>
            </div>

            <form @submit.prevent="register" class="needs-validation" novalidate>
              <!-- Имя -->
              <div class="mb-4">
                <label for="firstName" class="form-label fw-medium text-dark-emphasis">Имя</label>
                <input v-model="firstName" id="firstName" type="text" class="form-control form-control-lg" required>
              </div>

              <!-- Фамилия -->
              <div class="mb-4">
                <label for="lastName" class="form-label fw-medium text-dark-emphasis">Фамилия</label>
                <input v-model="lastName" id="lastName" type="text" class="form-control form-control-lg" required>
              </div>

              <!-- Email -->
              <div class="mb-4">
                <label for="email" class="form-label fw-medium text-dark-emphasis">Email</label>
                <input v-model="email" id="email" type="email" class="form-control form-control-lg" required>
              </div>

              <!-- Пароль -->
              <div class="mb-4">
                <label for="password" class="form-label fw-medium text-dark-emphasis">Пароль</label>
                <input v-model="password" id="password" type="password" class="form-control form-control-lg" required>
                <div class="form-text">Минимум 6 символов</div>
              </div>

              <!-- Кнопка -->
              <div class="d-grid mb-4">
                <button type="submit" class="btn btn-lg fw-bold btn-login text-white">
                  Зарегистрироваться
                </button>
              </div>
            </form>
          </div>
        </div>

        <div class="text-center mt-4">
          <p class="text-muted small">
            Уже есть аккаунт?
            <router-link to="/login" class="text-decoration-none fw-medium link-green">Войти</router-link>
          </p>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'

const router = useRouter()
const firstName = ref('')
const lastName = ref('')
const email = ref('')
const password = ref('')

async function register() {
  const res = await fetch(import.meta.env.VITE_API_URL + '/Auth/register', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({
      firstName: firstName.value,
      lastName: lastName.value,
      email: email.value,
      password: password.value
    })
  })

  if (!res.ok) {
    alert('Ошибка регистрации')
    return
  }

  const data = await res.json()
  localStorage.setItem('auth', JSON.stringify(data))
  router.push('/') // после регистрации сразу на главную
}
</script>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Montserrat:wght@300;400;500;600;700&family=Inter:wght@300;400;500;600&display=swap');
</style>
