<template>
  <div class="app">
    <nav class="nav">
      <router-link to="/">Главная</router-link>
      <router-link to="/login">Вход</router-link>
      <router-link to="/me">Профиль</router-link>
      <router-link to="/my-services">Мои услуги</router-link>
      <router-link to="/my-invoices">Мои счета</router-link>
      <router-link to="/admin">Админ</router-link>
      <router-link to="/admin/users">Пользователи</router-link>
    </nav>
    <main class="content">
      <router-view />
    </main>
  </div>
</template>

<style>
.nav { display: flex; gap: 12px; padding: 10px; border-bottom: 1px solid #eee; }
.nav a { text-decoration: none; }
.content { padding: 16px; }
</style>
