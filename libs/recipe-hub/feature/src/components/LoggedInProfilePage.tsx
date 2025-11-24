import {
  Button,
  handleError,
  Icon,
  selectPicturesForMode,
  Spinner,
  SpinnerButton,
} from '@micra-pro/shared/ui';
import { Component, createSignal, For, Setter, Show } from 'solid-js';
import {
  createRecipesAccessor,
  createUserAccessor,
} from '@micra-pro/recipe-hub/data-access';
import { twMerge } from 'tailwind-merge';
import { T, useTranslationContext } from '../generated/language-types';
import {
  EspressoProperties,
  fetchRoasteriesLevel,
  V60Properties,
} from '@micra-pro/bean-management/data-access';
import { BeanSelectorDialog } from '@micra-pro/bean-management/feature';
import moment from 'moment';
import { UserPage } from './UserPage';
import picturesImport from '../generated/pictures-import';

type Tabs = 'user' | 'espresso' | 'v60';

export const LoggedInProfilePage: Component<{
  logout: () => void;
  username: string;
}> = (props) => {
  const { t } = useTranslationContext();
  const userAccessor = createUserAccessor();
  const recipeAccessor = createRecipesAccessor(
    () => userAccessor.user()?.id ?? '',
  );
  const localRoasteries = fetchRoasteriesLevel();
  const [page, setPage] = createSignal<Tabs>('espresso');
  const [beanSelector, setBeanSelector] = createSignal(false);
  const [isAdding, setIsAdding] = createSignal(false);
  const roasteries = () =>
    localRoasteries
      .roasteries()
      .map((r) => ({
        ...r,
        beans: r.beans
          .filter((b) =>
            b.recipes.find(
              (r) =>
                r.__typename ===
                (page() === 'espresso'
                  ? 'EspressoProperties'
                  : 'V60Properties'),
            ),
          )
          .filter(
            (b) =>
              !(
                page() === 'espresso' &&
                recipeAccessor.espresso().find((r) => r.recipe.id === b.id)
              ) &&
              !(
                page() === 'v60' &&
                recipeAccessor.v60().find((r) => r.recipe.id === b.id)
              ),
          ),
      }))
      .filter((r) => r.beans.length > 0);
  const localRecipes = () =>
    localRoasteries
      .roasteries()
      .flatMap((r) => r.beans)
      .filter((b) =>
        page() === 'espresso'
          ? b.recipes.find((r) => r.__typename === 'EspressoProperties')
          : b.recipes.find((r) => r.__typename === 'V60Properties'),
      )
      .map((b) => b.id);
  const onBeanSelected = (id: string) => {
    setBeanSelector(false);
    const selectedPage = page();
    const roastery = localRoasteries
      .roasteries()
      .find((r) => r.beans.find((b) => b.id === id));
    if (!roastery) return;
    const bean = roastery.beans.find((b) => b.id === id);
    if (!bean) return;
    const recipe = bean.recipes.find(
      (r) =>
        r.__typename ===
        (selectedPage === 'espresso' ? 'EspressoProperties' : 'V60Properties'),
    );
    if (!recipe) return;
    setIsAdding(true);
    if (selectedPage === 'espresso') {
      const recipeProperties = recipe.properties as EspressoProperties;
      recipeAccessor
        .addEspresso({
          id: bean.id,
          beanName: bean.properties.name,
          roastery: roastery.properties.name,
          brewTemperature: recipeProperties.brewTemperature,
          coffeeQuantity: recipeProperties.coffeeQuantity,
          grindSetting: recipeProperties.grindSetting,
          inCupQuantity: recipeProperties.inCupQuantity,
          targetExtractionTimeInSeconds: moment
            .duration(recipeProperties.targetExtractionTime)
            .asSeconds(),
        })
        .then((s) => {
          if (!s) handleError({ title: t('failed'), message: '' });
        })
        .finally(() => setIsAdding(false));
    } else if (selectedPage === 'v60') {
      const recipeProperties = recipe.properties as V60Properties;
      recipeAccessor
        .addV60({
          id: bean.id,
          beanName: bean.properties.name,
          roastery: roastery.properties.name,
          brewTemperature: recipeProperties.brewTemperature,
          coffeeQuantity: recipeProperties.coffeeQuantity,
          grindSetting: recipeProperties.grindSetting,
          inCupQuantity: recipeProperties.inCupQuantity,
        })
        .then((s) => {
          if (!s) handleError({ title: t('failed'), message: '' });
        })
        .finally(() => setIsAdding(false));
    } else setIsAdding(false);
  };
  const add = () => setBeanSelector(true);
  return (
    <div class="flex h-full w-full flex-col">
      <BeanSelectorDialog
        isOpen={beanSelector()}
        onClose={() => setBeanSelector(false)}
        roasteries={roasteries()}
        onBeanSelected={onBeanSelected}
      />
      <div class="flex h-12 w-full border-b">
        <div class="mr-2 flex h-full w-full gap-2">
          <Tab tab="user" current={page()} setTab={setPage} />
          <Tab tab="espresso" current={page()} setTab={setPage} />
          <Tab tab="v60" current={page()} setTab={setPage} />
        </div>
        <div class="flex h-full items-center py-1">
          <Button
            variant="outline"
            class="flex h-full items-center justify-center rounded-lg inset-shadow-sm"
            onClick={props.logout}
          >
            <Icon iconName="logout" />
          </Button>
        </div>
      </div>
      <div class="flex h-full w-full flex-col overflow-x-hidden">
        <Show when={page() === 'user'}>
          <Show when={userAccessor.user()}>
            {(user) => (
              <UserPage
                user={user()}
                delete={userAccessor.delete}
                changePassword={userAccessor.changePassword}
                createMfaSecret={userAccessor.createMfaSecret}
                setMfa={userAccessor.setMfa}
              />
            )}
          </Show>
        </Show>
        <Show
          when={
            page() === 'espresso' && !isAdding() && !recipeAccessor.loading()
          }
        >
          <RecipesTable
            recipes={recipeAccessor.espresso()}
            add={add}
            localRecipes={localRecipes()}
          />
        </Show>
        <Show
          when={page() === 'v60' && !isAdding() && !recipeAccessor.loading()}
        >
          <RecipesTable
            recipes={recipeAccessor.v60()}
            add={add}
            localRecipes={localRecipes()}
          />
        </Show>
        <Show
          when={
            (isAdding() || recipeAccessor.loading()) &&
            (page() === 'espresso' || page() === 'v60')
          }
        >
          <div class="flex h-full w-full items-center justify-center">
            <Spinner class="h-20 w-20" />
          </div>
        </Show>
        <Show when={userAccessor.loading() && page() === 'user'}>
          <div class="flex h-full w-full items-center justify-center">
            <Spinner class="h-20 w-20" />
          </div>
        </Show>
      </div>
    </div>
  );
};

const Tab: Component<{
  tab: Tabs;
  current: Tabs;
  setTab: (tab: Tabs) => void;
}> = (props) => {
  const pictures = selectPicturesForMode(picturesImport);
  return (
    <div
      class={twMerge(
        'flex h-full w-1/3 items-center justify-center rounded-t-xl border-t border-r border-l',
        props.current === props.tab ? 'bg-secondary inset-shadow-sm' : '',
      )}
      onClick={() => props.setTab(props.tab)}
    >
      <div class="h-full py-2">
        <img src={pictures()[props.tab]} class="h-full object-scale-down" />
      </div>
    </div>
  );
};

export const RecipesTable: Component<{
  recipes: {
    recipe: {
      roastery: string;
      beanName: string;
      id: string;
    };
    delete: () => Promise<boolean>;
    update: () => Promise<boolean>;
  }[];
  localRecipes: string[];
  add: () => void;
}> = (props) => {
  const [deleting, setDeleting] = createSignal<string[]>([]);
  const [updating, setUpdating] = createSignal<string[]>([]);
  const { t } = useTranslationContext();
  const action = (
    func: () => Promise<boolean>,
    id: string,
    setState: Setter<string[]>,
  ) => {
    setState((d) => d.concat(id));
    func()
      .then((s) => {
        if (!s) handleError({ title: t('failed'), message: '' });
      })
      .finally(() => setState((d) => d.filter((i) => i !== id)));
  };
  return (
    <div class="no-scrollbar flex h-full flex-col rounded-b-md border-r border-b border-l text-base inset-shadow-sm">
      <div class="flex h-full w-full flex-col">
        <div class="flex h-12 w-full border-b font-bold shadow-xs">
          <div class="flex w-full text-sm">
            <div class="flex w-1/2 items-center border-r px-2">
              <T key="roastery" />
            </div>
            <div class="flex w-1/2 items-center px-2">
              <T key="bean" />
            </div>
          </div>
          <div class="flex h-full w-1/4 items-center justify-center px-2 py-2">
            <Button class="h-full w-full" onClick={props.add}>
              <Icon iconName="add" />
            </Button>
          </div>
        </div>
        <div class="no-scrollbar h-full w-full overflow-x-hidden overflow-y-scroll">
          <For each={props.recipes}>
            {(r) => (
              <div class="flex h-10 min-h-10 border-b">
                <div class="flex w-full text-sm">
                  <div class="flex w-1/2 items-center border-r px-2">
                    {r.recipe.roastery}
                  </div>
                  <div class="flex w-1/2 items-center px-2">
                    {r.recipe.beanName}
                  </div>
                </div>
                <div class="flex h-full w-1/4 items-center justify-center gap-2 px-2 py-1">
                  <SpinnerButton
                    class="h-full w-10 px-0"
                    loading={updating().includes(r.recipe.id)}
                    onClick={() => action(r.update, r.recipe.id, setUpdating)}
                    variant="outline"
                    disabled={
                      !props.localRecipes.includes(r.recipe.id) ||
                      updating().includes(r.recipe.id) ||
                      deleting().includes(r.recipe.id)
                    }
                  >
                    <Icon iconName="upload" />
                  </SpinnerButton>
                  <SpinnerButton
                    class="h-full w-10 px-0"
                    loading={deleting().includes(r.recipe.id)}
                    onClick={() => action(r.delete, r.recipe.id, setDeleting)}
                    variant="outline"
                    disabled={
                      updating().includes(r.recipe.id) ||
                      deleting().includes(r.recipe.id)
                    }
                  >
                    <Icon iconName="delete" />
                  </SpinnerButton>
                </div>
              </div>
            )}
          </For>
        </div>
      </div>
    </div>
  );
};
