<template>
  <div class="container py-5">
    <div class="row justify-content-center">
      <div class="col-12 col-md-8 col-lg-6 col-xl-5">
        <!-- Карточка с анимированным градиентным фоном -->
        <div class="card login-card shadow-lg border-0 overflow-hidden">
          <!-- Декоративный верхний элемент -->
          <div class="position-absolute top-0 start-0 w-100">
            <div class="bg-gradient-top"></div>
          </div>
          
          <div class="card-body p-4 p-md-5 position-relative z-1">
            <!-- Заголовок с иконкой -->
            <div class="text-center mb-5">
              <div class="icon-wrapper mb-3">
                <svg width="48" height="48" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
                  <path d="M12 12C14.7614 12 17 9.76142 17 7C17 4.23858 14.7614 2 12 2C9.23858 2 7 4.23858 7 7C7 9.76142 9.23858 12 12 12Z" fill="var(--color-green-deep)"/>
                  <path d="M12 14C7.58172 14 4 17.5817 4 22H20C20 17.5817 16.4183 14 12 14Z" fill="var(--color-green-dark)"/>
                </svg>
              </div>
              <h1 class="h2 fw-bold mb-2 text-dark-emphasis">Добро пожаловать</h1>
              <p class="text-muted">Войдите в свой аккаунт</p>
            </div>

            <!-- Форма входа -->
            <form @submit.prevent="login" class="needs-validation" novalidate>
              <!-- Поле email -->
              <div class="mb-4">
                <label for="email" class="form-label fw-medium text-dark-emphasis">
                  <i class="bi bi-envelope me-2"></i>Email адрес
                </label>
                <div class="input-group">
                  <span class="input-group-text bg-transparent border-end-0">
                    <i class="bi bi-at text-muted"></i>
                  </span>
                  <input 
                    v-model="email" 
                    type="email" 
                    id="email"
                    class="form-control form-control-lg border-start-0 ps-0"
                    placeholder="name@example.com"
                    required
                  >
                </div>
                <div class="form-text">Введите ваш email для входа</div>
              </div>

              <!-- Поле пароля -->
              <div class="mb-4">
                <label for="password" class="form-label fw-medium text-dark-emphasis">
                  <i class="bi bi-key me-2"></i>Пароль
                </label>
                <div class="input-group">
                  <span class="input-group-text bg-transparent border-end-0">
                    <i class="bi bi-lock text-muted"></i>
                  </span>
                  <input 
                    v-model="password" 
                    type="password" 
                    id="password"
                    class="form-control form-control-lg border-start-0 ps-0"
                    placeholder="Введите пароль"
                    required
                  >
                </div>
                <div class="form-text">Минимум 6 символов</div>
              </div>

              <!-- Кнопка входа -->
              <div class="d-grid mb-4">
                <button 
                  type="submit" 
                  class="btn btn-lg fw-bold btn-login text-white"
                >
                  <i class="bi bi-box-arrow-in-right me-2"></i>
                  Войти в систему
                </button>
              </div>

              <!-- Дополнительная информация -->
              <div class="text-center mt-4 pt-3 border-top">
                <p class="small text-muted mb-0">
                  Продолжая, вы соглашаетесь с нашими 
                  <a href="#" class="text-decoration-none fw-medium link-green">Условиями</a> и 
                  <a href="#" class="text-decoration-none fw-medium link-green">Политикой конфиденциальности</a>
                </p>
              </div>
            </form>
          </div>
        </div>

        <!-- Футер -->
        <div class="text-center mt-4">
          <p class="text-muted small">
            Нет аккаунта? 
            <a href="#" class="text-decoration-none fw-medium link-green">Зарегистрироваться</a>
          </p>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import {ref} from 'vue'
import {useRouter, useRoute} from 'vue-router'

const router = useRouter()
const route = useRoute()
const email = ref('')
const password = ref('')


async function login(event: SubmitEvent) {
  event.preventDefault()
  const res = await fetch(import.meta.env.VITE_API_URL + '/Auth/login', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ email: email.value, password: password.value })
  })
  if (!res.ok) throw new Error('Ошибка авторизации')
  const auth = await res.json()
  localStorage.setItem('token', auth.token)
  localStorage.setItem('role', auth.role)

  // загружаем профиль
  const me = await fetch(import.meta.env.VITE_API_URL + '/Users/me', {
    headers: { 'Authorization': 'Bearer ' + auth.token }
  })
  if (!me.ok) throw new Error('Не удалось загрузить профиль')
  const profile = await me.json()
  localStorage.setItem('profile', JSON.stringify(profile))

  const redirect = (route.query.redirect as string) ?? (auth.role === 'admin' ? '/admin' : '/')
  router.push(redirect)
}


</script>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Montserrat:wght@300;400;500;600;700&family=Inter:wght@300;400;500;600&display=swap');

:root {
  --color-mint: #E6F4EC;
  --color-green-light: #A3D9A5;
  --color-green-deep: #4B8F6B;
  --color-green-dark: #2F5D3A;
  --color-yellow-green: #C4D96F;
  --color-warm-warning: #D9A76F;
  
  --font-heading: 'Montserrat', sans-serif;
  --font-body: 'Inter', sans-serif;
  
  --shadow-soft: 0 4px 20px rgba(0, 0, 0, 0.08);
  --shadow-medium: 0 8px 30px rgba(0, 0, 0, 0.12);
  --radius-large: 20px;
  --radius-medium: 12px;
}

body {
  font-family: var(--font-body);
  background: linear-gradient(135deg, var(--color-mint) 0%, #f8f9fa 100%);
  min-height: 100vh;
}

.login-card {
  border-radius: var(--radius-large);
  background: rgba(255, 255, 255, 0.95);
  backdrop-filter: blur(10px);
  border: 1px solid rgba(255, 255, 255, 0.2);
  position: relative;
  overflow: hidden;
  transition: transform 0.3s ease;
}

.login-card:hover {
  transform: translateY(-5px);
}

.bg-gradient-top {
  height: 6px;
  background: linear-gradient(90deg, 
    var(--color-green-light) 0%,
    var(--color-green-deep) 25%,
    var(--color-yellow-green) 50%,
    var(--color-warm-warning) 75%,
    var(--color-green-dark) 100%);
}

.icon-wrapper {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  width: 80px;
  height: 80px;
  border-radius: 50%;
  background: linear-gradient(135deg, var(--color-green-light) 0%, var(--color-mint) 100%);
  box-shadow: var(--shadow-soft);
}

h1, h2, h3, .h2 {
  font-family: var(--font-heading);
  font-weight: 700;
  color: var(--color-green-dark);
}

.form-label {
  font-family: var(--font-heading);
  font-weight: 600;
  font-size: 0.95rem;
  margin-bottom: 0.5rem;
  color: var(--color-green-dark);
}

.input-group {
  border: 2px solid var(--color-green-light);
  border-radius: var(--radius-medium);
  background: white;
  transition: all 0.3s ease;
}

.input-group:focus-within {
  border-color: var(--color-green-deep);
  box-shadow: 0 0 0 0.25rem rgba(75, 143, 107, 0.15);
}

.input-group-text {
  border: none;
  background: transparent;
  color: var(--color-green-deep);
}

.form-control,
.form-select {
  border: none;
  background: transparent;
  font-family: var(--font-body);
  font-weight: 400;
  padding-left: 0.5rem;
}

.form-control:focus,
.form-select:focus {
  box-shadow: none;
  background: transparent;
}

.form-control-lg {
  font-size: 1rem;
  padding: 0.75rem 0.5rem;
}

.form-text {
  font-size: 0.85rem;
  color: var(--color-green-deep);
  margin-top: 0.25rem;
  font-family: var(--font-body);
}

.btn-login {
  background: linear-gradient(135deg, var(--color-green-dark) 0%, var(--color-green-deep) 100%);
  border: none;
  border-radius: var(--radius-medium);
  padding: 0.875rem 1.5rem;
  font-family: var(--font-heading);
  font-weight: 600;
  letter-spacing: 0.5px;
  transition: all 0.3s ease;
  position: relative;
  overflow: hidden;
}

.btn-login:hover {
  transform: translateY(-2px);
  box-shadow: 0 6px 20px rgba(47, 93, 58, 0.3);
}

.btn-login:active {
  transform: translateY(0);
}

.btn-login::after {
  content: '';
  position: absolute;
  top: 0;
  left: -100%;
  width: 100%;
  height: 100%;
  background: linear-gradient(90deg, transparent, rgba(255, 255, 255, 0.2), transparent);
  transition: left 0.5s ease;
}

.btn-login:hover::after {
  left: 100%;
}

.link-green {
  color: var(--color-green-deep);
  position: relative;
  transition: color 0.3s ease;
}

.link-green:hover {
  color: var(--color-green-dark);
}

.link-green::after {
  content: '';
  position: absolute;
  bottom: -2px;
  left: 0;
  width: 0;
  height: 2px;
  background: var(--color-yellow-green);
  transition: width 0.3s ease;
}

.link-green:hover::after {
  width: 100%;
}

.text-muted {
  color: #6c757d !important;
}

.border-top {
  border-color: var(--color-green-light) !important;
}

/* Анимация появления */
@keyframes fadeIn {
  from {
    opacity: 0;
    transform: translateY(20px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.login-card {
  animation: fadeIn 0.6s ease-out;
}

/* Кастомные стили для селекта */
.form-select {
  cursor: pointer;
  appearance: none;
  background-image: url("data:image/svg+xml,%3csvg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 16 16'%3e%3cpath fill='none' stroke='%234B8F6B' stroke-linecap='round' stroke-linejoin='round' stroke-width='2' d='m2 5 6 6 6-6'/%3e%3c/svg%3e");
  background-repeat: no-repeat;
  background-position: right 0.75rem center;
  background-size: 16px 12px;
}

/* Адаптивность */
@media (max-width: 768px) {
  .card-body {
    padding: 2rem !important;
  }
  
  .login-card {
    margin-top: 1rem;
  }
}
</style>