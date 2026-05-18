import {
  Component,
  createEffect,
  createSignal,
  Match,
  Show,
  Switch,
} from 'solid-js';
import { AlertDialog, AlertDialogContent } from '@micra-pro/shared/ui';
import { Spout } from '@micra-pro/brew-by-weight/data-access';
import { BrewByWeightContent } from './BrewByWeightContent';
import { useSelectedSpoutContext } from './SpoutSelectorContextProvider';
import { BrewByTimeContent } from './BrewByTimeContent';

export type BrewMethod = 'time' | 'weight';

export const BrewByWeightPannel: Component<{
  recipe: {
    beanId: string;
    coffeeQuantity: number;
    grindSetting: number;
    inCupQuantity: number;
    targetExtractionTime: string;
  } | null;
  method: BrewMethod | undefined;
  onClose: () => void;
}> = (props) => {
  const [recipe, setRecipe] = createSignal<
    | {
        beanId: string;
        coffeeQuantity: number;
        grindSetting: number;
        inCupQuantity: number;
        targetExtractionTime: string;
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
              <Switch>
                <Match when={props.method === 'weight'}>
                  <BrewByWeightContent recipe={r()} onClose={props.onClose} />
                </Match>
                <Match when={props.method === 'time'}>
                  <BrewByTimeContent recipe={r()} onClose={props.onClose} />
                </Match>
              </Switch>
            )}
          </Show>
        </AlertDialogContent>
      </AlertDialog>
    </>
  );
};
