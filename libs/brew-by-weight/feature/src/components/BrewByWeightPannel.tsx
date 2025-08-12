import { Component, createEffect, createSignal, Show } from 'solid-js';
import { AlertDialog, AlertDialogContent } from '@micra-pro/shared/ui';
import { Spout } from '@micra-pro/brew-by-weight/data-access';
import { BrewByWeightContent } from './BrewByWeightContent';
import { useSelectedSpoutContext } from './SpoutSelectorContextProvider';

export const BrewByWeightPannel: Component<{
  recipe: {
    beanId: string;
    coffeeQuantity: number;
    grindSetting: number;
    inCupQuantity: number;
    targetExtractionTime: string;
    flowProfile?: {
      startFlow: number;
      dataPoints: { flow: number; time: any }[];
    };
  } | null;
  onClose: () => void;
}> = (props) => {
  const [recipe, setRecipe] = createSignal<
    | {
        beanId: string;
        coffeeQuantity: number;
        grindSetting: number;
        inCupQuantity: number;
        targetExtractionTime: string;
        flowProfile?: {
          startFlow: number;
          dataPoints: { flow: number; time: any }[];
        };
        spout: Spout;
      }
    | undefined
  >();
  const spoutContext = useSelectedSpoutContext();
  createEffect(() => {
    const recipe = props.recipe;
    if (recipe) setRecipe({ ...recipe, spout: spoutContext.selectedSpout() });
    else setRecipe(undefined);
  });
  return (
    <>
      <AlertDialog open={!!recipe()}>
        <AlertDialogContent
          onOpenAutoFocus={(e) => e.preventDefault()}
          onInteractOutside={(e) => e.preventDefault()}
          class="border-none bg-transparent shadow-none"
        >
          <Show when={recipe()}>
            {(r) => (
              <BrewByWeightContent recipe={r()} onClose={props.onClose} />
            )}
          </Show>
        </AlertDialogContent>
      </AlertDialog>
    </>
  );
};
