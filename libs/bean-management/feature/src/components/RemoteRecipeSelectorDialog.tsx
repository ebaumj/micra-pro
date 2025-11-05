import {
  EspressoProperties,
  V60Properties,
} from '@micra-pro/bean-management/data-access';
import { fetchRecipes } from '@micra-pro/recipe-hub/data-access';
import {
  Button,
  Dialog,
  DialogContent,
  Icon,
  Spinner,
  TextField,
  TextFieldRoot,
} from '@micra-pro/shared/ui';
import moment from 'moment';
import { Component, For, Show } from 'solid-js';
import { createStore } from 'solid-js/store';
import { useTranslationContext } from '../generated/language-types';

const TextInput: Component<{
  text: string;
  set: (text: string) => void;
  placeholder?: string;
}> = (props) => (
  <TextFieldRoot value={props.text} onChange={props.set}>
    <TextField placeholder={props.placeholder} />
  </TextFieldRoot>
);

function SharedRemoteRecipeSelectorDialogContent<T>(props: {
  recipes: (T & { username: string; roastery: string; beanName: string })[];
  onRecipeSelected?: (recipe: T) => void;
}) {
  const { t } = useTranslationContext();
  const [filter, setFilter] = createStore({
    username: '',
    roastery: '',
    beanName: '',
  });
  const filteredRecipes = () =>
    props.recipes.filter(
      (r) =>
        r.username
          .toLowerCase()
          .includes(filter.username.toLocaleLowerCase()) &&
        r.roastery.toLowerCase().includes(filter.roastery.toLowerCase()) &&
        r.beanName.toLowerCase().includes(filter.beanName.toLowerCase()),
    );
  const resetFilter = () => {
    setFilter('username', '');
    setFilter('roastery', '');
    setFilter('beanName', '');
  };
  return (
    <div class="flex h-80 w-full flex-col px-2 pt-4 pb-2">
      <div>
        <div class="flex w-full py-1 text-base">
          <div class="flex w-1/3 items-center overflow-hidden pr-1 whitespace-nowrap">
            <TextInput
              text={filter.username}
              set={(t: string) => setFilter('username', t)}
              placeholder={t('username')}
            />
          </div>
          <div class="flex w-1/3 items-center overflow-hidden px-1 whitespace-nowrap">
            <TextInput
              text={filter.roastery}
              set={(t: string) => setFilter('roastery', t)}
              placeholder={t('roastery')}
            />
          </div>
          <div class="flex w-1/3 items-center overflow-hidden px-1 whitespace-nowrap">
            <TextInput
              text={filter.beanName}
              set={(t: string) => setFilter('beanName', t)}
              placeholder={t('bean-name')}
            />
          </div>
          <div class="px-1">
            <Button variant="outline" onClick={resetFilter}>
              <Icon iconName="remove_selection" />
            </Button>
          </div>
        </div>
      </div>
      <div class="no-scrollbar h-full overflow-y-scroll rounded-md border text-sm inset-shadow-sm">
        <For each={filteredRecipes()}>
          {(r) => (
            <div class="flex w-full border-b py-1">
              <div class="flex w-1/3 items-center overflow-hidden border-r px-2 whitespace-nowrap">
                {r.username}
              </div>
              <div class="flex w-1/3 items-center overflow-hidden border-r px-2 whitespace-nowrap">
                {r.roastery}
              </div>
              <div class="flex w-1/3 items-center overflow-hidden px-2 whitespace-nowrap">
                {r.beanName}
              </div>
              <div class="px-1">
                <Button onClick={() => props.onRecipeSelected?.(r)}>
                  <Icon iconName="download" />
                </Button>
              </div>
            </div>
          )}
        </For>
      </div>
    </div>
  );
}

const RemoteRecipeSelectorDialogContent: Component<{
  isOpenEspresso: boolean;
  isOpenV60: boolean;
  onEspressoSelected?: (recipe: EspressoProperties) => void;
  onV60Selected?: (recipe: V60Properties) => void;
}> = (props) => {
  const recipes = fetchRecipes();
  return (
    <>
      <Show when={props.isOpenEspresso && !recipes.loading()}>
        <SharedRemoteRecipeSelectorDialogContent<EspressoProperties>
          recipes={recipes.espresso().map((e) => ({
            ...e,
            targetExtractionTime: moment
              .duration(e.targetExtractionTimeInSeconds, 'seconds')
              .toISOString(),
          }))}
          onRecipeSelected={props.onEspressoSelected}
        />
      </Show>
      <Show when={props.isOpenV60 && !recipes.loading()}>
        <SharedRemoteRecipeSelectorDialogContent<V60Properties>
          recipes={recipes.v60()}
          onRecipeSelected={props.onV60Selected}
        />
      </Show>
      <Show when={recipes.loading()}>
        <div class="flex h-80 w-full flex-col items-center justify-center px-2 pt-4 pb-2">
          <Spinner class="h-20 w-20" />
        </div>
      </Show>
    </>
  );
};

export const RemoteRecipeSelectorDialog: Component<{
  isOpenEspresso: boolean;
  isOpenV60: boolean;
  close: () => void;
  onEspressoSelected?: (recipe: EspressoProperties) => void;
  onV60Selected?: (recipe: V60Properties) => void;
}> = (props) => (
  <Dialog
    open={props.isOpenEspresso || props.isOpenV60}
    onOpenChange={(o) => (o ? undefined : props.close())}
  >
    <DialogContent
      onOpenAutoFocus={(e) => e.preventDefault()}
      onInteractOutside={(e) => e.preventDefault()}
    >
      <RemoteRecipeSelectorDialogContent {...props} />
    </DialogContent>
  </Dialog>
);
