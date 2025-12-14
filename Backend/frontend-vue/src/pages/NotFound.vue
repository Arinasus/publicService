<template>
  <div class="not-found-page">
    <!-- Фоновые декоративные элементы -->
    <div class="background-elements">
      <div class="circle circle-1"></div>
      <div class="circle circle-2"></div>
      <div class="circle circle-3"></div>
      <div class="leaf leaf-1"></div>
      <div class="leaf leaf-2"></div>
      <div class="leaf leaf-3"></div>
    </div>

    <div class="container-fluid min-vh-100 d-flex align-items-center">
      <div class="row justify-content-center w-100">
        <div class="col-12 col-md-10 col-lg-8 col-xl-6">
          <!-- Основная карточка -->
          <div class="card not-found-card shadow-lg border-0 overflow-hidden">
            <div class="card-body p-4 p-md-5 text-center position-relative z-2">
              
              <!-- Анимированное число 404 -->
              <div class="error-number mb-4">
                <span class="digit digit-1">4</span>
                <span class="digit digit-2">0</span>
                <span class="digit digit-3">4</span>
              </div>

              <!-- Иконка с анимацией -->
              <div class="icon-wrapper mb-4">
                <div class="lost-icon">
                  <svg width="80" height="80" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
                    <path d="M12 22C17.5228 22 22 17.5228 22 12C22 6.47715 17.5228 2 12 2C6.47715 2 2 6.47715 2 12C2 17.5228 6.47715 22 12 22Z" stroke="var(--color-green-deep)" stroke-width="2"/>
                    <path d="M15 9L9 15" stroke="var(--color-green-deep)" stroke-width="2" stroke-linecap="round"/>
                    <path d="M9 9L15 15" stroke="var(--color-green-deep)" stroke-width="2" stroke-linecap="round"/>
                    <path d="M12 8V12" stroke="var(--color-warm-warning)" stroke-width="2" stroke-linecap="round"/>
                    <path d="M12 16H12.01" stroke="var(--color-warm-warning)" stroke-width="2" stroke-linecap="round"/>
                  </svg>
                </div>
              </div>

              <!-- Заголовок -->
              <h1 class="display-5 fw-bold mb-3">Страница не найдена</h1>
              
              <!-- Описание -->
              <p class="lead text-muted mb-4">
                Упс! Кажется, мы потеряли эту страницу. Возможно, она была перемещена, 
                удалена или вы ввели неверный адрес.
              </p>

              <!-- Дополнительный контекст -->
              <div class="info-box p-3 mb-4 rounded-3">
                <div class="d-flex align-items-center justify-content-center">
                  <i class="bi bi-info-circle fs-4 me-2 text-warning"></i>
                  <p class="mb-0 small">
                    Проверьте URL на наличие опечаток или воспользуйтесь поиском
                  </p>
                </div>
              </div>

              <!-- Действия -->
              <div class="action-buttons row g-3 justify-content-center">
                <div class="col-12 col-sm-auto">
                  <button @click="goBack" class="btn btn-lg btn-outline-primary btn-action">
                    <i class="bi bi-arrow-left me-2"></i>
                    Вернуться назад
                  </button>
                </div>
                <div class="col-12 col-sm-auto">
                  <router-link to="/" class="btn btn-lg btn-primary btn-action">
                    <i class="bi bi-house-door me-2"></i>
                    На главную
                  </router-link>
                </div>
                <div class="col-12 col-sm-auto">
                  <button @click="searchHelp" class="btn btn-lg btn-outline-success btn-action">
                    <i class="bi bi-search me-2"></i>
                    Поиск помощи
                  </button>
                </div>
              </div>

              <!-- Полезные ссылки -->
              <div class="useful-links mt-5 pt-4 border-top">
                <h6 class="mb-3 fw-semibold">Полезные ссылки:</h6>
                <div class="row g-3 justify-content-center">
                  <div class="col-auto">
                    <router-link to="/help" class="link-item">
                      <i class="bi bi-question-circle me-1"></i> Помощь
                    </router-link>
                  </div>
                  <div class="col-auto">
                    <router-link to="/faq" class="link-item">
                      <i class="bi bi-chat-left-text me-1"></i> FAQ
                    </router-link>
                  </div>
                  <div class="col-auto">
                    <router-link to="/contact" class="link-item">
                      <i class="bi bi-envelope me-1"></i> Контакты
                    </router-link>
                  </div>
                  <div class="col-auto">
                    <a href="#" @click.prevent="reportIssue" class="link-item">
                      <i class="bi bi-bug me-1"></i> Сообщить об ошибке
                    </a>
                  </div>
                </div>
              </div>

              <!-- Поиск -->
              <div class="search-box mt-4">
                <div class="input-group input-group-lg">
                  <input 
                    type="text" 
                    class="form-control" 
                    placeholder="Поиск по сайту..."
                    v-model="searchQuery"
                    @keyup.enter="performSearch"
                  >
                  <button class="btn btn-success" type="button" @click="performSearch">
                    <i class="bi bi-search"></i>
                  </button>
                </div>
              </div>

            </div>
            
            <!-- Градиентная полоса внизу -->
            <div class="card-footer bg-transparent border-0 py-3">
              <div class="gradient-bar"></div>
            </div>
          </div>

          <!-- Котировка -->
          <div class="quote-box text-center mt-4">
            <p class="text-muted small mb-0">
              <i class="bi bi-quote me-1"></i>
              Иногда чтобы найти правильный путь, нужно сначала заблудиться
            </p>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'

const router = useRouter()
const searchQuery = ref('')

function goBack() {
  router.go(-1)
}
function searchHelp() {
  router.push('/help?search=' + encodeURIComponent(searchQuery.value))
}

function performSearch() {
  if (searchQuery.value.trim()) {
    router.push('/search?q=' + encodeURIComponent(searchQuery.value))
  }
}

function reportIssue() {
  // Здесь можно добавить логику для отправки отчета об ошибке
  alert('Спасибо за сообщение! Мы уже работаем над этой проблемой.')
}
</script>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Montserrat:wght@300;400;500;600;700;800&family=Inter:wght@300;400;500;600&display=swap');

.not-found-page {
  min-height: 100vh;
  background: linear-gradient(135deg, 
    var(--color-mint) 0%,
    rgba(163, 217, 165, 0.2) 50%,
    var(--color-mint) 100%);
  position: relative;
  overflow: hidden;
  padding: 2rem 0;
}

/* Фоновые элементы */
.background-elements {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  pointer-events: none;
  z-index: 0;
}

.circle {
  position: absolute;
  border-radius: 50%;
  opacity: 0.1;
  animation: float 20s infinite ease-in-out;
}

.circle-1 {
  width: 300px;
  height: 300px;
  background: var(--color-green-deep);
  top: 10%;
  left: 5%;
  animation-delay: 0s;
}

.circle-2 {
  width: 200px;
  height: 200px;
  background: var(--color-yellow-green);
  top: 60%;
  right: 10%;
  animation-delay: 5s;
}

.circle-3 {
  width: 150px;
  height: 150px;
  background: var(--color-warm-warning);
  bottom: 10%;
  left: 20%;
  animation-delay: 10s;
}

.leaf {
  position: absolute;
  opacity: 0.15;
  animation: spin 30s infinite linear;
}

.leaf-1 {
  width: 100px;
  height: 100px;
  background: var(--color-green-light);
  clip-path: polygon(50% 0%, 0% 100%, 100% 100%);
  top: 20%;
  right: 15%;
  animation-delay: 0s;
}

.leaf-2 {
  width: 80px;
  height: 80px;
  background: var(--color-green-dark);
  clip-path: polygon(50% 0%, 0% 100%, 100% 100%);
  bottom: 30%;
  right: 25%;
  animation-delay: 15s;
}

.leaf-3 {
  width: 60px;
  height: 60px;
  background: var(--color-yellow-green);
  clip-path: polygon(50% 0%, 0% 100%, 100% 100%);
  top: 40%;
  left: 10%;
  animation-delay: 7s;
}

@keyframes float {
  0%, 100% {
    transform: translateY(0) rotate(0deg);
  }
  33% {
    transform: translateY(-20px) rotate(120deg);
  }
  66% {
    transform: translateY(20px) rotate(240deg);
  }
}

@keyframes spin {
  from {
    transform: rotate(0deg);
  }
  to {
    transform: rotate(360deg);
  }
}

/* Карточка */
.not-found-card {
  border-radius: 24px;
  background: rgba(255, 255, 255, 0.95);
  backdrop-filter: blur(10px);
  border: 1px solid rgba(255, 255, 255, 0.3);
  position: relative;
  z-index: 1;
  overflow: visible;
}

.not-found-card::before {
  content: '';
  position: absolute;
  top: -2px;
  left: -2px;
  right: -2px;
  bottom: -2px;
  background: linear-gradient(45deg, 
    var(--color-green-light),
    var(--color-green-deep),
    var(--color-yellow-green),
    var(--color-warm-warning));
  border-radius: 26px;
  z-index: -1;
  opacity: 0.3;
  filter: blur(10px);
}

/* Анимация числа 404 */
.error-number {
  display: flex;
  justify-content: center;
  gap: 1.5rem;
  margin-bottom: 2rem;
}

.digit {
  font-family: 'Montserrat', sans-serif;
  font-weight: 900;
  font-size: 8rem;
  color: var(--color-green-dark);
  display: inline-block;
  text-shadow: 
    4px 4px 0px rgba(47, 93, 58, 0.1),
    8px 8px 0px rgba(47, 93, 58, 0.05);
  animation: bounce 2s infinite;
}

.digit-1 {
  animation-delay: 0s;
}

.digit-2 {
  animation-delay: 0.2s;
}

.digit-3 {
  animation-delay: 0.4s;
}

@keyframes bounce {
  0%, 100% {
    transform: translateY(0);
  }
  50% {
    transform: translateY(-20px);
  }
}

/* Иконка */
.icon-wrapper {
  margin: 2rem 0;
}

.lost-icon {
  display: inline-block;
  animation: shake 3s infinite;
}

@keyframes shake {
  0%, 100% {
    transform: rotate(0deg);
  }
  25% {
    transform: rotate(-5deg);
  }
  75% {
    transform: rotate(5deg);
  }
}

/* Заголовки */
h1 {
  font-family: 'Montserrat', sans-serif;
  font-weight: 800;
  background: linear-gradient(135deg, 
    var(--color-green-dark),
    var(--color-green-deep));
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
}

/* Инфо-бокс */
.info-box {
  background: linear-gradient(135deg, 
    rgba(230, 244, 236, 0.5),
    rgba(163, 217, 165, 0.3));
  border: 1px solid var(--color-green-light);
  max-width: 600px;
  margin: 0 auto;
}

/* Кнопки */
.btn-action {
  font-family: 'Montserrat', sans-serif;
  font-weight: 600;
  padding: 0.75rem 2rem;
  border-radius: 12px;
  transition: all 0.3s ease;
  min-width: 200px;
}

.btn-primary {
  background: linear-gradient(135deg, 
    var(--color-green-dark),
    var(--color-green-deep));
  border: none;
}

.btn-primary:hover {
  transform: translateY(-3px);
  box-shadow: 0 10px 25px rgba(47, 93, 58, 0.3);
}

.btn-outline-primary {
  color: var(--color-green-dark);
  border: 2px solid var(--color-green-deep);
}

.btn-outline-primary:hover {
  background: var(--color-green-dark);
  color: white;
  transform: translateY(-3px);
}

.btn-outline-success {
  color: var(--color-green-deep);
  border: 2px solid var(--color-yellow-green);
}

.btn-outline-success:hover {
  background: var(--color-yellow-green);
  color: var(--color-green-dark);
  transform: translateY(-3px);
}

/* Полезные ссылки */
.useful-links {
  border-top: 1px solid rgba(163, 217, 165, 0.3);
}

.link-item {
  text-decoration: none;
  color: var(--color-green-deep);
  font-weight: 500;
  padding: 0.5rem 1rem;
  border-radius: 8px;
  transition: all 0.3s ease;
  display: inline-block;
}

.link-item:hover {
  background: rgba(163, 217, 165, 0.2);
  color: var(--color-green-dark);
  transform: translateX(5px);
}

/* Поиск */
.search-box {
  max-width: 500px;
  margin: 0 auto;
}

.search-box .form-control {
  border: 2px solid var(--color-green-light);
  border-right: none;
  border-radius: 12px 0 0 12px;
}

.search-box .form-control:focus {
  border-color: var(--color-green-deep);
  box-shadow: 0 0 0 0.25rem rgba(75, 143, 107, 0.15);
}

.search-box .btn {
  border-radius: 0 12px 12px 0;
  background: var(--color-green-deep);
  border-color: var(--color-green-deep);
}

/* Градиентная полоса */
.gradient-bar {
  height: 4px;
  background: linear-gradient(90deg, 
    var(--color-green-light) 0%,
    var(--color-green-deep) 33%,
    var(--color-yellow-green) 66%,
    var(--color-warm-warning) 100%);
  border-radius: 2px;
  width: 80%;
  margin: 0 auto;
}

/* Цитата */
.quote-box {
  font-style: italic;
  opacity: 0.8;
}

/* Адаптивность */
@media (max-width: 768px) {
  .digit {
    font-size: 5rem;
  }
  
  .error-number {
    gap: 1rem;
  }
  
  .btn-action {
    min-width: 100%;
  }
  
  .action-buttons .col-sm-auto {
    width: 100%;
  }
}

@media (max-width: 576px) {
  .digit {
    font-size: 4rem;
  }
  
  h1 {
    font-size: 2rem;
  }
  
  .card-body {
    padding: 2rem !important;
  }
}
</style>