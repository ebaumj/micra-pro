<template>
  <div class="flex h-screen w-screen flex-col">
    <div class="absolute flex h-12 w-full items-center justify-center">
      <a
        href="/"
        class="mt-6 flex items-center justify-center rounded-lg border border-gray-500 bg-gray-200 px-4 py-2 font-bold shadow-lg shadow-gray-500"
        >Home</a
      >
    </div>
    <iframe class="h-full w-full" v-bind:src="link" />
  </div>
</template>

<script setup lang="ts">
const link = useRuntimeConfig().dummyFrontendPage;
</script>
