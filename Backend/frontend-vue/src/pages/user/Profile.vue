<script setup lang="ts">
import { ref } from 'vue'
const me = ref({ firstName: 'Арина', lastName: 'Сельвич', email: 'Ariolla@example.com' })
</script>

<template>
  <div>
    <h2>Профиль</h2>
    <p>{{ me.firstName }} {{ me.lastName }}</p>
    <p>{{ me.email }}</p>
  </div>
</template>
<!-- docker compose exec postgres psql -U util_user -d utilities
-->