<template>
  <div class="app">
    <nav class="nav">
      <router-link to="/">Главная</router-link>
      <router-link to="/login">Вход</router-link>
      <router-link to="/me">Профиль</router-link>
      <router-link to="/my-services">Мои услуги</router-link>
      <router-link to="/my-invoices">Мои счета</router-link>
      <router-link to="/my-notifications">Мои уведомления</router-link>
      <router-link to="/admin">Админ</router-link>
      <router-link to="/admin/users">Пользователи</router-link>
      <router-link to="/admin/services">Услуги</router-link>
      <router-link to="/admin/invoices">Счета</router-link>
    </nav>
    <main class="content">
      <router-view />
    </main>
  </div>
</template>

<style scoped>
.nav {
  display: flex;
  gap: 12px;
  padding: 10px;
  border-bottom: 1px solid #eee;
}
.nav a {
  text-decoration: none;
  color: #0078d7;
  font-weight: 500;
}
.nav a.router-link-active {
  text-decoration: underline;
}
.content {
  padding: 16px;
}
</style>
