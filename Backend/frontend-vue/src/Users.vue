<script setup lang="ts">
import {ref, onMounted} from 'vue'

const users = ref<any[]>([])

onMounted(async ()=>{
    const res = await fetch(import.meta.env.VITE_API_URL + '/Users')
    users.value = await res.json()
})
</script>

<template>
    <div>
        <h2>Список пользователей</h2>
        <ul>
            <li v-for="u in users" :key="u.userID">
                {{ u.firstName }}{{ u.lastName }} - {{ u.email }}
            </li>
        </ul>
    </div>
</template>