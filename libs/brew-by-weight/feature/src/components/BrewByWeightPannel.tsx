import { Component, createEffect, createSignal, Show } from 'solid-js';
import {
  AlertDialog,
  AlertDialogContent,
  Button,
  Sheet,
  SheetContent,
  SheetDescription,
  SheetHeader,
  SheetTitle,
} from '@micra-pro/shared/ui';
import { Spout } from '@micra-pro/brew-by-weight/data-access';
import { T } from '../generated/language-types';
import { BrewByWeightContent } from './BrewByWeightContent';

export const BrewByWeightPannel: Component<{
  recipe: {
    beanId: string;
    coffeeQuantity: number;
    grindSetting: number;
    inCupQuantity: number;
    scaleId: string;
    targetExtractionTime: string;
  } | null;
  onClose: () => void;
}> = (props) => {
  const [recipe, setRecipe] = createSignal<
    | {
        beanId: string;
        coffeeQuantity: number;
        grindSetting: number;
        inCupQuantity: number;
        scaleId: string;
        targetExtractionTime: string;
        spout: Spout;
      }
    | undefined
  >();
  createEffect(() => {
    if (!props.recipe) setRecipe(undefined);
  });
  return (
    <>
      <Sheet
        open={!!props.recipe && !recipe()}
        onOpenChange={(o) => (o ? undefined : props.onClose())}
      >
        <SheetContent onOpenAutoFocus={(e) => e.preventDefault()} class="w-80">
          <SheetHeader>
            <SheetTitle>
              <T key="select-spout" />
            </SheetTitle>
            <SheetDescription class="flex flex-col gap-4 pt-4">
              <Button
                variant="outline"
                class="flex h-14 w-full justify-center py-2"
                onClick={() =>
                  setRecipe({ spout: Spout.Single, ...props.recipe! })
                }
              >
                <T key="spout-single" />
              </Button>
              <Button
                variant="outline"
                class="flex h-14 w-full justify-center py-2"
                onClick={() =>
                  setRecipe({ spout: Spout.Double, ...props.recipe! })
                }
              >
                <T key="spout-double" />
              </Button>
              <Button
                variant="outline"
                class="flex h-14 w-full justify-center py-2"
                onClick={() =>
                  setRecipe({ spout: Spout.Naked, ...props.recipe! })
                }
              >
                <T key="spout-naked" />
              </Button>
            </SheetDescription>
          </SheetHeader>
        </SheetContent>
      </Sheet>
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
