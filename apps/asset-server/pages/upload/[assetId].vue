<template>
  <div class="flex h-screen w-screen items-center justify-center p-8">
    <label
      for="img"
      class="h-full max-h-60 w-full max-w-60 rounded-lg bg-slate-100 shadow-lg"
      ><img src="~/assets/images/icon.png" class="h-full w-full object-contain"
    /></label>
    <input
      class="absolute hidden"
      type="file"
      id="img"
      accept="image/*"
      @change="(event) => handelFileUpload(event)"
    />
  </div>
</template>

<script setup>
const route = useRoute();
const { assetId } = route.params;
const token = route.query.token;
const handelFileUpload = async (event) => {
  const files = event.target.files;
  if (!files || files.length === 0) return;
  const file = files[0];
  var fd = new FormData();
  fd.append('fname', file.name);
  fd.append('data', file);
  try {
    const { data } = await useFetch(`/api/assets/${assetId}`, {
      method: 'POST',
      body: fd,
      headers: { authorization: `Bearer ${token}` },
    });
    await navigateTo(`/upload?success=${data.value}`);
  } catch {
    await navigateTo(`/upload`);
  }
};
</script>
