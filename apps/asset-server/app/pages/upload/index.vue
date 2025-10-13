<template>
  <div class="flex h-screen w-screen items-center justify-center bg-white">
    <div
      v-if="success === 'true'"
      class="h-12 w-12 rounded-full bg-green-500"
    />
    <div v-else class="h-12 w-12 rounded-full bg-red-500" />
  </div>
</template>

<script setup lang="ts">
const success = useRoute().query.success;
</script>
