<template>
  <div
    class="flex h-screen w-screen flex-col items-center justify-center bg-gray-100 p-8"
  >
    <div class="flex h-full w-full flex-col items-center justify-center">
      <div v-if="pending" class="flex max-h-60 items-center justify-center">
        <svg
          aria-hidden="true"
          class="inline h-full w-full animate-spin rounded-full fill-gray-600 p-4 text-gray-200 dark:fill-gray-300 dark:text-gray-600"
          viewBox="0 0 100 101"
          fill="none"
          xmlns="http://www.w3.org/2000/svg"
        >
          <path
            d="M100 50.5908C100 78.2051 77.6142 100.591 50 100.591C22.3858 100.591 0 78.2051 0 50.5908C0 22.9766 22.3858 0.59082 50 0.59082C77.6142 0.59082 100 22.9766 100 50.5908ZM9.08144 50.5908C9.08144 73.1895 27.4013 91.5094 50 91.5094C72.5987 91.5094 90.9186 73.1895 90.9186 50.5908C90.9186 27.9921 72.5987 9.67226 50 9.67226C27.4013 9.67226 9.08144 27.9921 9.08144 50.5908Z"
            fill="currentColor"
          />
          <path
            d="M93.9676 39.0409C96.393 38.4038 97.8624 35.9116 97.0079 33.5539C95.2932 28.8227 92.871 24.3692 89.8167 20.348C85.8452 15.1192 80.8826 10.7238 75.2124 7.41289C69.5422 4.10194 63.2754 1.94025 56.7698 1.05124C51.7666 0.367541 46.6976 0.446843 41.7345 1.27873C39.2613 1.69328 37.813 4.19778 38.4501 6.62326C39.0873 9.04874 41.5694 10.4717 44.0505 10.1071C47.8511 9.54855 51.7191 9.52689 55.5402 10.0491C60.8642 10.7766 65.9928 12.5457 70.6331 15.2552C75.2735 17.9648 79.3347 21.5619 82.5849 25.841C84.9175 28.9121 86.7997 32.2913 88.1811 35.8758C89.083 38.2158 91.5421 39.6781 93.9676 39.0409Z"
            fill="currentFill"
          />
        </svg>
      </div>
      <div
        v-else-if="error"
        class="flex h-full w-full items-center justify-center rounded-lg border border-red-300 bg-red-50"
      >
        <div class="text-red-500">Error loading data</div>
      </div>
      <div v-else class="flex h-full w-full flex-col">
        <div
          class="flex rounded-t-lg border border-b-0 border-gray-700 bg-gray-900 p-3 text-blue-400"
        >
          <div class="flex h-8 w-full items-center font-mono text-sm">
            <span class="text-purple-400">function&nbsp;</span>
            <span class="text-yellow-200">{{ webhookName }}</span
            >(<span class="text-blue-200">machineEvent</span>,&nbsp;
            <span class="text-blue-200">timestamp</span>) {
          </div>
          <div class="flex h-full gap-2">
            <button
              :disabled="!canDelete"
              @click="deleteContent"
              class="flex h-8 w-8 items-center justify-center rounded-full border bg-red-600 shadow-md transition hover:bg-red-700 disabled:opacity-50"
            >
              <svg
                xmlns="http://www.w3.org/2000/svg"
                class="ml-0.5 h-5 w-5 text-white"
                viewBox="0 0 24 24"
                fill="currentColor"
              >
                <path
                  d="M6 19c0 1.1.9 2 2 2h8c1.1 0 2-.9 2-2V7H6v12zM19 4h-3.5l-1-1h-5l-1 1H5v2h14V4z"
                />
              </svg>
            </button>
            <button
              :disabled="!canSave"
              @click="saveContent"
              class="flex h-8 w-8 items-center justify-center rounded-full border bg-blue-600 shadow-md transition hover:bg-blue-700 disabled:opacity-50"
            >
              <svg
                xmlns="http://www.w3.org/2000/svg"
                class="ml-0.5 h-5 w-5 text-white"
                viewBox="0 0 24 24"
                fill="currentColor"
              >
                <path
                  d="M17 3H5c-1.11 0-2 .9-2 2v14c0 1.1.89 2 2 2h14c1.1 0 2-.9 2-2V7l-4-4zm-5 16c-1.66 0-3-1.34-3-3s1.34-3 3-3 3 1.34 3 3-1.34 3-3 3zm3-10H5V5h10v4z"
                />
              </svg>
            </button>
            <button
              @click="runContent"
              class="flex h-8 w-8 items-center justify-center rounded-full border bg-green-600 shadow-md transition hover:bg-green-700 disabled:opacity-50"
            >
              <svg
                xmlns="http://www.w3.org/2000/svg"
                class="ml-0.5 h-5 w-5 text-white"
                viewBox="0 0 24 24"
                fill="currentColor"
              >
                <path d="M8 5v14l11-7z" />
              </svg>
            </button>
          </div>
        </div>
        <div class="h-full w-full border border-gray-700">
          <vue-monaco-editor
            v-model:value="webhookContent"
            theme="vs-dark"
            language="javascript"
            :options="editorOptions"
            @mount="handleEditorMount"
          />
        </div>
        <div
          class="rounded-b-lg border border-t-0 border-gray-700 bg-gray-900 p-3 font-mono text-sm text-gray-400"
        >
          }
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue';
import { VueMonacoEditor } from '@guolao/vue-monaco-editor';

const refreshTime = 30 * 60 * 1000;

const route = useRoute();
const webhookName = route.params.webhookName as string;
let token = route.query.token;
let refreshToken = route.query.refreshToken;

const webhookContent = ref('');
const savedWebhookContent = ref<string | null>(null);
const { data, pending, error } = useFetch<{
  content: string;
  eventFormat: string;
}>(`/api/webhook/${webhookName}`, {
  lazy: true,
  headers: { authorization: `Bearer ${token}` },
});

watch(
  data,
  (newData) => {
    if (newData) {
      webhookContent.value = newData.content ?? '';
      savedWebhookContent.value = newData.content;
    }
  },
  { immediate: true },
);

const canSave = computed(
  () =>
    savedWebhookContent.value !== webhookContent.value &&
    !pending.value &&
    !error.value,
);

const canDelete = computed(
  () => savedWebhookContent.value !== null && !pending.value && !error.value,
);

const saveContent = async () => {
  try {
    const saved = await $fetch<{ content: string }>(
      `/api/webhook/${webhookName}`,
      {
        method: 'POST',
        body: {
          content: webhookContent.value,
        },
        headers: { authorization: `Bearer ${token}` },
      },
    );
    savedWebhookContent.value = saved.content;
  } catch (e) {
    console.error(e);
    alert('Failed to save snippet');
  }
};

const deleteContent = async () => {
  try {
    await $fetch<{ content: string }>(`/api/webhook/${webhookName}`, {
      method: 'DELETE',
      headers: { authorization: `Bearer ${token}` },
    });
    savedWebhookContent.value = null;
    webhookContent.value = '';
  } catch (e) {
    console.error(e);
    alert('Failed to delete snippet');
  }
};

const runContent = () => {
  try {
    const combinedCode = `${data.value?.eventFormat}\n${webhookContent.value}`;
    const userFunction = new Function(combinedCode);
    userFunction();
  } catch (err) {
    console.error('Error executing snippet:', err);
    if (err instanceof Error) {
      alert('Error executing code: ' + err.message);
    }
  }
};

const editorOptions = {
  automaticLayout: true,
  minimap: { enabled: false },
  fontSize: 14,
  scrollBeyondLastLine: false,
  padding: { top: 16, bottom: 16 },
};

const handleEditorMount = (_: any, monaco: any) => {
  const format = data.value?.eventFormat;
  if (format) {
    monaco.languages.typescript.javascriptDefaults.addExtraLib(
      format,
      `ts:filename/${webhookName}.d.ts`,
    );
  }
  monaco.languages.typescript.javascriptDefaults.setDiagnosticsOptions({
    noSemanticValidation: false,
    noSyntaxValidation: false,
  });
};

const refreshTokens = async () => {
  try {
    const refreshed = await $fetch<{ token: string; refreshToken: string }>(
      `/api/token/${webhookName}`,
      {
        method: 'POST',
        body: {
          refreshToken: refreshToken,
        },
        headers: { authorization: `Bearer ${token}` },
      },
    );
    token = refreshed.token;
    refreshToken = refreshed.refreshToken;
  } catch (e) {
    console.error(e);
    alert('Failed to refresh tokens');
  }
  setTimeout(refreshTokens, refreshTime);
};
setTimeout(refreshTokens, refreshTime);
</script>
