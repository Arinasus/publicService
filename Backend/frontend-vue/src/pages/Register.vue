<template>
  <div class="auth-wrapper fade-in">
    <div class="auth-card shadow-lg">

      <!-- Заголовок -->
      <div class="auth-header text-center">
        <i class="bi bi-house-heart-fill logo-icon"></i>
        <h1 class="title">Коммунальные услуги</h1>
        <p class="subtitle">Удобный сервис для управления коммунальными платежами</p>
      </div>

      <!-- Форма регистрации -->
      <form @submit.prevent="register" class="auth-form">

        <div class="row">
          <div class="col-md-6">
            <div class="form-group">
              <label>Имя</label>
              <input v-model="firstName" type="text" class="form-control" placeholder="Введите имя" required>
            </div>
          </div>

          <div class="col-md-6">
            <div class="form-group">
              <label>Фамилия</label>
              <input v-model="lastName" type="text" class="form-control" placeholder="Введите фамилию" required>
            </div>
          </div>
        </div>

        <div class="form-group">
          <label>Email</label>
          <input v-model="email" type="email" class="form-control" placeholder="example@domain.com" required>
          <div class="form-hint">Используется для входа в систему</div>
        </div>

        <div class="form-group">
          <label>Пароль</label>
          <input v-model="password" type="password" class="form-control" placeholder="Придумайте пароль" required>
          <div class="form-hint">Минимум 6 символов</div>
        </div>

        <div class="form-check terms">
          <input class="form-check-input" type="checkbox" id="terms" required>
          <label class="form-check-label" for="terms">
            Я согласен с <a href="#">Условиями использования</a> и <a href="#">Политикой конфиденциальности</a>
          </label>
        </div>

        <button type="submit" class="btn-primary-auth">
          Создать аккаунт
        </button>

        <div class="divider">
          <span>Уже зарегистрированы?</span>
        </div>

        <router-link to="/login" class="btn-outline-auth">
          Войти
        </router-link>

      </form>

      <!-- Футер -->
      <div class="auth-footer">
        <div><i class="bi bi-shield-check me-1"></i> Безопасно</div>
        <div><i class="bi bi-clock-history me-1"></i> 24/7 доступ</div>
        <div><i class="bi bi-phone me-1"></i> Мобильная версия</div>
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
    headers: {
      'Content-Type': 'application/json'
    },
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
  
  localStorage.removeItem('auth')
  router.push('/login')
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
  background: var(--color-bg);
  padding: 20px;
}

.auth-card {
  width: 100%;
  max-width: 480px;
  background: white;
  border-radius: var(--radius-lg);
  padding: 40px 35px;
  box-shadow: var(--shadow-md);
}

.auth-header .logo-icon {
  font-size: 3rem;
  color: var(--color-accent);
}

.title {
  font-size: 1.9rem;
  font-weight: 700;
  margin-top: 10px;
  color: var(--color-navy);
}

.subtitle {
  color: var(--color-muted);
  margin-top: 5px;
  font-size: 1rem;
}

.form-group {
  margin-bottom: 22px;
}

.form-group label {
  font-weight: 600;
  color: var(--color-navy);
  margin-bottom: 6px;
  display: block;
}

.form-control {
  width: 100%;
  padding: 12px 14px;
  border-radius: var(--radius-md);
  border: 2px solid #e0e0e0;
  transition: var(--transition);
}

.form-control:focus {
  border-color: var(--color-accent);
  box-shadow: 0 0 0 3px rgba(202, 62, 73, 0.15);
}

.form-hint {
  font-size: 0.85rem;
  color: var(--color-muted);
  margin-top: 4px;
}

.terms {
  margin: 20px 0;
}

.terms a {
  color: var(--color-accent);
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
  margin-top: 10px;
}

.btn-primary-auth:hover {
  transform: translateY(-2px);
  box-shadow: var(--shadow-md);
}

.divider {
  text-align: center;
  margin: 25px 0;
  position: relative;
}

.divider span {
  background: white;
  padding: 0 12px;
  color: var(--color-muted);
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

.btn-outline-auth {
  display: block;
  width: 100%;
  padding: 12px;
  border-radius: var(--radius-md);
  border: 2px solid var(--color-navy);
  color: var(--color-navy);
  text-align: center;
  font-weight: 600;
  transition: var(--transition);
}

.btn-outline-auth:hover {
  background: var(--color-navy);
  color: white;
  transform: translateY(-2px);
}

.auth-footer {
  margin-top: 30px;
  font-size: 0.85rem;
  color: var(--color-muted);
  display: flex;
  justify-content: space-between;
}

</style>