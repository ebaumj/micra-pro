<template>
  <div
    class="flex h-screen w-screen flex-col items-center justify-center p-8 text-lg"
  >
    <img
      src="~/assets/images/machine.jpeg"
      class="h-full max-h-60 w-full max-w-60 rounded-2xl object-contain shadow-xl"
    />
    <div class="flex gap-4">
      <a
        class="mt-6 flex items-center justify-center rounded-lg bg-slate-200 px-4 py-2 text-slate-800 shadow-md"
        href="/dummy"
        >Dummy UI</a
      >
      <a
        class="mt-6 flex items-center justify-center rounded-lg bg-slate-200 px-4 py-2 text-slate-800 shadow-md"
        href="https://github.com/ebaumj/micra-pro"
        >Application Repository</a
      >
      <a
        class="mt-6 flex items-center justify-center rounded-lg bg-slate-200 px-4 py-2 text-slate-800 shadow-md"
        href="https://github.com/ebaumj/micra-pro-builder"
        >Image Builder Repository</a
      >
    </div>
    <div
      v-if="images.length > 0"
      class="mt-6 flex flex-col gap-2 rounded-md border bg-slate-800 p-4 text-slate-200 shadow-inner"
    >
      <p class="border-b pb-2 text-xl font-bold">
        Prebuild Images (Raspberry Pi 5)
      </p>
      <a
        v-for="image in images"
        class="cursor-pointer hover:underline"
        v-bind:href="image.link"
        target="_blank"
      >
        Image <span class="font-bold"> v{{ image.version }} </span>,
        {{ formatDate(image.created_at) }}
      </a>
    </div>
  </div>
</template>

<script setup lang="ts">
const images = ref([] as { link: string; version: string; created_at: Date }[]);
try {
  const { data } = await useFetch(`/image`);
  if (data.value)
    images.value = data.value.images.map((i) => ({
      ...i,
      created_at: new Date(i.created_at),
    }));
} catch {}
const twoNumbers = (value: number) => (value < 10 ? `0${value}` : `${value}`);
const formatDate = (date: Date) =>
  `${twoNumbers(date.getDate())}.${twoNumbers(date.getMonth() + 1)}.${date.getFullYear()}`;
</script>
