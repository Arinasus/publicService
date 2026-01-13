<template>
  <div class="auth-wrapper fade-in">
    <div class="auth-card shadow-lg">

      <!-- Логотип / Заголовок -->
      <div class="auth-header text-center">
        <i class="bi bi-house-heart-fill logo-icon"></i>
        <h1 class="title">Коммунальные услуги</h1>
        <p class="subtitle">Управление коммунальными платежами онлайн</p>
      </div>

      <!-- Форма -->
      <form @submit.prevent="login" class="auth-form">

        <!-- Email -->
        <div class="form-group">
          <label>Email</label>
          <input 
            v-model="email"
            type="email"
            class="form-control"
            placeholder="example@domain.com"
            required
          >
        </div>

        <!-- Пароль -->
        <div class="form-group">
          <label>Пароль</label>
          <input 
            v-model="password"
            type="password"
            class="form-control"
            placeholder="Введите пароль"
            required
          >
          <div class="forgot">
            <a href="#">Забыли пароль?</a>
          </div>
        </div>

        <!-- Кнопка -->
        <button 
  type="submit" 
  class="btn-login-blue"
>
  <i class="bi bi-box-arrow-in-right me-2"></i>
  Войти
</button>

        <!-- Регистрация -->
        <div class="register-block">
          <p>Нет аккаунта?</p>
          <router-link to="/register" class="btn-register">
            Создать аккаунт
          </router-link>
        </div>

      </form>

      <!-- Футер -->
      <div class="auth-footer">
        <div><i class="bi bi-shield-check me-1"></i> Безопасный вход</div>
        <div><i class="bi bi-clock-history me-1"></i> 24/7 Доступ</div>
        <div><i class="bi bi-phone me-1"></i> Мобильное приложение</div>
      </div>

    </div>
  </div>
</template>


<script setup lang="ts">
import {ref} from 'vue'
import {useRouter} from 'vue-router'

const router = useRouter()
const email = ref('')
const password = ref('')

async function login() {
  const res = await fetch(import.meta.env.VITE_API_URL + '/Auth/login', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({
      email: email.value,
      password: password.value
    })
  })

  if (!res.ok) {
    alert('Ошибка входа')
    return
  }

  const data = await res.json()
  localStorage.setItem('auth', JSON.stringify({
    token: data.token,
    role: data.role
  }))

  const redirect = router.currentRoute.value.query.redirect as string | undefined
  router.push(redirect || '/')
}
</script>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Montserrat:wght@300;400;500;600;700&family=Inter:wght@300;400;500;600&display=swap');
@import url('https://cdn.jsdelivr.net/npm/bootstrap-icons@1.10.0/font/bootstrap-icons.css');

.auth-wrapper {
  min-height: 100vh;
  display: flex;
  justify-content: center;
  align-items: center;
  background: #f3f4f7;
  padding: 20px;
  font-family: 'Inter', sans-serif;
}

.auth-card {
  width: 100%;
  max-width: 420px;
  background: white;
  border-radius: 18px;
  padding: 40px 35px;
  animation: fadeIn 0.5s ease-out;
}

.auth-header .logo-icon {
  font-size: 3rem;
  color: #ca3e49;
}
.btn-login-blue {
  width: 100%;
  padding: 12px;
  border-radius: 10px;
  background: linear-gradient(135deg, #2f3650, #3a4569);
  border: none;
  color: white;
  font-weight: 600;
  transition: 0.25s;
  box-shadow: var(--shadow-sm);
}

.btn-login-blue:hover {
  background: linear-gradient(135deg, #3a4569, #2f3650);
  transform: translateY(-2px);
  box-shadow: var(--shadow-md);
}

.title {
  font-size: 1.9rem;
  font-weight: 700;
  margin-top: 10px;
  color: #2f3650;
}

.subtitle {
  color: #6c757d;
  margin-top: 5px;
  font-size: 1rem;
}

.form-group {
  margin-bottom: 22px;
}

.form-group label {
  font-weight: 600;
  color: #2f3650;
  margin-bottom: 6px;
  display: block;
}

.form-control {
  width: 100%;
  padding: 12px 14px;
  border-radius: 10px;
  border: 2px solid #e0e0e0;
  transition: 0.25s;
  font-size: 0.95rem;
}

.form-control:focus {
  border-color: #ca3e49;
  box-shadow: 0 0 0 3px rgba(202, 62, 73, 0.15);
}

.forgot {
  margin-top: 6px;
  text-align: right;
}

.forgot a {
  font-size: 0.85rem;
  color: #ca3e49;
}

.btn-primary-auth {
  width: 100%;
  padding: 12px;
  border-radius: 10px;
  background: linear-gradient(135deg, #ca3e49, #e04a55);
  border: none;
  color: white;
  font-weight: 600;
  margin-top: 5px;
  transition: 0.25s;
}

.btn-primary-auth:hover {
  transform: translateY(-2px);
  box-shadow: 0 6px 20px rgba(202, 62, 73, 0.25);
}

.divider {
  text-align: center;
  margin: 25px 0;
  position: relative;
}

.divider span {
  background: white;
  padding: 0 12px;
  color: #6c757d;
  font-size: 0.9rem;
}

.divider::before {
  content: "";
  position: absolute;
  top: 50%;
  left: 0;
  height: 1px;
  width: 100%;
  background: #ddd;
  z-index: -1;
}

.btn-alt {
  width: 100%;
  padding: 10px;
  border-radius: 10px;
  border: 2px solid #2f3650;
  background: white;
  color: #2f3650;
  margin-bottom: 12px;
  transition: 0.25s;
}

.btn-alt:hover {
  background: #2f3650;
  color: white;
  transform: translateY(-2px);
}

.register-block {
  text-align: center;
  margin-top: 20px;
}

.btn-register {
  display: inline-block;
  padding: 10px 25px;
  border-radius: 10px;
  border: 2px solid #ca3e49;
  color: #ca3e49;
  transition: 0.25s;
}

.btn-register:hover {
  background: #ca3e49;
  color: white;
}

.auth-footer {
  margin-top: 30px;
  font-size: 0.85rem;
  color: #6c757d;
  display: flex;
  justify-content: space-between;
}

</style>