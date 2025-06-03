import { Component, createSignal, For, Show } from 'solid-js';
import {
  BeanProperties,
  createBeansAccessor,
  createRecipesAccessor,
  createRoasteriesAccessor,
  EspressoProperties,
  RoasteryProperties,
  V60Properties,
} from '@micra-pro/bean-management/data-access';
import {
  AlertDialog,
  AlertDialogContent,
  Button,
  Icon,
  LongPressDiv,
  Spinner,
} from '@micra-pro/shared/ui';
import { twMerge } from 'tailwind-merge';
import {
  EditRoasteryDialog,
  EditRoasteryDialogContent,
} from './EditRoasteryDialog';
import { EditBeanDialog, EditBeanDialogContent } from './EditBeanDialog';
import picturesImport from '../generated/pictures-import';
import {
  EditEspressoDialog,
  EditEspressoDialogContent,
} from './EditEspressoDialog';
import { EditV60Dialog, EditV60DialogContent } from './EditV60Dialog';

export const EditBeansPage: Component = () => {
  const roasteriesAccessor = createRoasteriesAccessor();
  const beansAccessor = createBeansAccessor();
  const recipesAccessor = createRecipesAccessor();

  const [selectedRoastery, setSelectedRoastery] = createSignal('');
  const [editRoasteryDialog, setEditRoasteryDialog] = createSignal<
    EditRoasteryDialogContent | undefined
  >();
  const [isAddingRoastery, setIsAddingRoastery] = createSignal(false);
  const selectRoastery = (id: string) =>
    setSelectedRoastery((r) => (r === id ? '' : id));
  const addRoastery = (properties: RoasteryProperties) => {
    setIsAddingRoastery(true);
    roasteriesAccessor.add(properties, () => {
      setEditRoasteryDialog(undefined);
      setIsAddingRoastery(false);
    });
  };

  const [selectedBean, setSelectedBean] = createSignal('');
  const [editBeanDialog, setEditBeanDialog] = createSignal<
    EditBeanDialogContent | undefined
  >();
  const [isAddingBean, setIsAddingBean] = createSignal(false);
  const selectBean = (id: string) =>
    setSelectedBean((b) => (b === id ? '' : id));
  const addBean = (roasteryId: string, properties: BeanProperties) => {
    setIsAddingBean(true);
    beansAccessor.add(roasteryId, properties, () => {
      setEditBeanDialog(undefined);
      setIsAddingBean(false);
    });
  };

  const currentEspressoRecipe = () =>
    recipesAccessor.recipesEspresso().find((r) => r.beanId === selectedBean());
  const [editEspressoDialog, setEditEspressoDialog] = createSignal<
    EditEspressoDialogContent | undefined
  >();
  const [isAddingEspresso, setIsAddingEspresso] = createSignal(false);
  const addEspresso = (beanId: string, properties: EspressoProperties) => {
    setIsAddingEspresso(true);
    recipesAccessor.addEspresso(beanId, properties, () => {
      setEditEspressoDialog(undefined);
      setIsAddingEspresso(false);
    });
  };

  const currentV60Recipe = () =>
    recipesAccessor.recipesV60().find((r) => r.beanId === selectedBean());
  const [editV60Dialog, setEditV60Dialog] = createSignal<
    EditV60DialogContent | undefined
  >();
  const [isAddingV60, setIsAddingV60] = createSignal(false);
  const addV60 = (beanId: string, properties: V60Properties) => {
    setIsAddingV60(true);
    recipesAccessor.addV60(beanId, properties, () => {
      setEditV60Dialog(undefined);
      setIsAddingV60(false);
    });
  };

  const isLoading = () =>
    roasteriesAccessor.isLoading() ||
    beansAccessor.isLoading() ||
    recipesAccessor.isLoading();

  return (
    <div class="no-scrollbar h-full w-full overflow-hidden">
      <AlertDialog open={isLoading()}>
        <AlertDialogContent class="flex items-center justify-center p-8">
          <Spinner class="h-20 w-20" />
        </AlertDialogContent>
      </AlertDialog>
      <EditRoasteryDialog
        content={editRoasteryDialog()}
        onClose={() => setEditRoasteryDialog(undefined)}
      />
      <EditBeanDialog
        content={editBeanDialog()}
        onClose={() => setEditBeanDialog(undefined)}
      />
      <EditEspressoDialog
        content={editEspressoDialog()}
        onClose={() => setEditEspressoDialog(undefined)}
      />
      <EditV60Dialog
        content={editV60Dialog()}
        onClose={() => setEditV60Dialog(undefined)}
      />
      <div
        class={twMerge(
          'flex h-full w-[150%] transition-transform duration-300',
          selectedBean() === '' ? '' : '-translate-x-1/3',
        )}
      >
        <div class="w-1/3 border-r">
          <div class="no-scrollbar flex h-full flex-col overflow-x-hidden overflow-y-scroll pr-6">
            <div class="w-full border-b" />
            <For each={roasteriesAccessor.roasteries()}>
              {(r) => (
                <LongPressDiv
                  class={twMerge(
                    'flex border-b active:bg-slate-100',
                    selectedRoastery() === r.id ? 'bg-slate-50' : '',
                  )}
                  onClick={() => selectRoastery(r.id)}
                  onLongPress={() =>
                    setEditRoasteryDialog({
                      properties: r.properties,
                      onSave: (props: RoasteryProperties) =>
                        r.update(props, () => setEditRoasteryDialog(undefined)),
                      onRemove: () =>
                        r.remove(() => setEditRoasteryDialog(undefined)),
                      isSaving: r.isUpdating,
                      isRemoving: r.isDeleting,
                    })
                  }
                >
                  <div class="flex h-full w-16 items-center justify-center">
                    <div class="pb-0.5">
                      <Icon class="text-3xl" iconName="folder" />
                    </div>
                  </div>
                  <div class="w-full gap-0 py-1">
                    <div class="overflow-hidden whitespace-nowrap text-base font-bold">
                      {r.properties.name}
                    </div>
                    <div class="overflow-hidden whitespace-nowrap text-xs">
                      {r.properties.location}
                    </div>
                  </div>
                  <div class="flex h-full w-14 items-center justify-center">
                    <Show when={selectedRoastery() === r.id}>
                      <div class="pb-0.5">
                        <Icon class="text-3xl" iconName="navigate_next" />
                      </div>
                    </Show>
                  </div>
                </LongPressDiv>
              )}
            </For>
            <div class="flex w-full items-center justify-center p-2">
              <Button
                class="flex h-8 w-20 items-center justify-center"
                variant="outline"
                onClick={() =>
                  setEditRoasteryDialog({
                    onSave: (props: RoasteryProperties) => addRoastery(props),
                    isSaving: isAddingRoastery,
                  })
                }
              >
                <Icon iconName="add" />
              </Button>
            </div>
          </div>
        </div>
        <div class="no-scrollbar flex h-full w-1/3 flex-col overflow-x-hidden overflow-y-scroll pl-6">
          <div class="w-full border-b" />
          <For
            each={beansAccessor
              .beans()
              .filter((b) => b.roasteryId === selectedRoastery())}
          >
            {(b) => (
              <LongPressDiv
                class={twMerge(
                  'flex border-b active:bg-slate-100',
                  selectedBean() === b.id ? 'bg-slate-50' : '',
                )}
                onClick={() => selectBean(b.id)}
                onLongPress={() =>
                  setEditBeanDialog({
                    properties: b.properties,
                    onSave: (props: BeanProperties) =>
                      b.update(props, () => setEditBeanDialog(undefined)),
                    onRemove: () =>
                      b.remove(() => setEditBeanDialog(undefined)),
                    isSaving: b.isUpdating,
                    isRemoving: b.isDeleting,
                  })
                }
              >
                <div class="flex w-20 items-center justify-center">
                  <img src={picturesImport.bean} class="h-full w-full p-3" />
                </div>
                <div class="w-full gap-0 overflow-hidden py-1">
                  <div class="overflow-hidden whitespace-nowrap text-base font-bold">
                    {b.properties.name}
                  </div>
                  <div class="overflow-hidden whitespace-nowrap text-xs">
                    {b.properties.countryCode}
                  </div>
                </div>
                <div class="flex w-14 items-center justify-center">
                  <Show when={selectedBean() === b.id}>
                    <div class="pb-0.5">
                      <Icon class="text-3xl" iconName="navigate_next" />
                    </div>
                  </Show>
                </div>
              </LongPressDiv>
            )}
          </For>
          <Show when={selectedRoastery() !== ''}>
            <div class="flex w-full items-center justify-center p-2">
              <Button
                class="flex h-8 w-20 items-center justify-center"
                variant="outline"
                onClick={() =>
                  setEditBeanDialog({
                    onSave: (props: BeanProperties) =>
                      addBean(selectedRoastery(), props),
                    isSaving: isAddingBean,
                  })
                }
              >
                <Icon iconName="add" />
              </Button>
            </div>
          </Show>
        </div>
        <div class="ml-6 flex w-1/3 flex-col border-l px-6">
          <div class="w-full border-b" />
          <div class="flex h-12 border-b">
            <div class="flex h-full w-16 items-center justify-center">
              <img
                src={picturesImport.espresso}
                class={twMerge(
                  'h-full w-full p-3',
                  currentEspressoRecipe() ? '' : 'opacity-50',
                )}
              />
            </div>
            <div class="w-full">
              <Show when={currentEspressoRecipe()}>
                <div class="flex h-full w-full items-center justify-center">
                  <Button
                    class="flex h-8 w-20 items-center justify-center"
                    variant="outline"
                    onClick={() =>
                      setEditEspressoDialog({
                        properties: currentEspressoRecipe()!.properties,
                        onSave: (properties: EspressoProperties) =>
                          currentEspressoRecipe()!.update(properties, () =>
                            setEditEspressoDialog(undefined),
                          ),
                        onRemove: () =>
                          currentEspressoRecipe()!.remove(() =>
                            setEditEspressoDialog(undefined),
                          ),
                        isSaving: () =>
                          currentV60Recipe()?.isUpdating() ?? false,
                        isRemoving: () =>
                          currentV60Recipe()?.isDeleting() ?? false,
                      })
                    }
                  >
                    <Icon iconName="edit" />
                  </Button>
                </div>
              </Show>
              <Show when={!currentEspressoRecipe()}>
                <div class="flex h-full w-full items-center justify-center">
                  <Button
                    class="flex h-8 w-20 items-center justify-center"
                    variant="outline"
                    onClick={() =>
                      setEditEspressoDialog({
                        onSave: (props: EspressoProperties) =>
                          addEspresso(selectedBean(), props),
                        isSaving: isAddingEspresso,
                      })
                    }
                  >
                    <Icon iconName="add" />
                  </Button>
                </div>
              </Show>
            </div>
          </div>
          <div class="flex h-12 border-b">
            <div class="flex h-full w-16 items-center justify-center">
              <img
                src={picturesImport.v60}
                class={twMerge(
                  'h-full w-full p-3',
                  currentV60Recipe() ? '' : 'opacity-50',
                )}
              />
            </div>
            <div class="w-full">
              <Show when={currentV60Recipe()}>
                <div class="flex h-full w-full items-center justify-center">
                  <Button
                    class="flex h-8 w-20 items-center justify-center"
                    variant="outline"
                    onClick={() =>
                      setEditV60Dialog({
                        properties: currentV60Recipe()!.properties,
                        onSave: (properties: V60Properties) =>
                          currentV60Recipe()!.update(properties, () =>
                            setEditV60Dialog(undefined),
                          ),
                        onRemove: () =>
                          currentV60Recipe()!.remove(() =>
                            setEditV60Dialog(undefined),
                          ),
                        isSaving: () =>
                          currentV60Recipe()?.isUpdating() ?? false,
                        isRemoving: () =>
                          currentV60Recipe()?.isDeleting() ?? false,
                      })
                    }
                  >
                    <Icon iconName="edit" />
                  </Button>
                </div>
              </Show>
              <Show when={!currentV60Recipe()}>
                <div class="flex h-full w-full items-center justify-center">
                  <Button
                    class="flex h-8 w-20 items-center justify-center"
                    variant="outline"
                    onClick={() =>
                      setEditV60Dialog({
                        onSave: (props: V60Properties) =>
                          addV60(selectedBean(), props),
                        isSaving: isAddingV60,
                      })
                    }
                  >
                    <Icon iconName="add" />
                  </Button>
                </div>
              </Show>
            </div>
          </div>
          <div class="flex w-full items-center justify-center p-2">
            <Button
              class="flex h-8 w-20 items-center justify-center"
              variant="outline"
              onClick={() => selectBean('')}
            >
              <Icon iconName="navigate_before" />
            </Button>
          </div>
        </div>
      </div>
    </div>
  );
};
