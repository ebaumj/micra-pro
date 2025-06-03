import { createContext, ParentComponent, useContext } from 'solid-js';

const DialogContext = createContext<{ mount: Node }>();

export const useDialogContext = () => {
  const context = useContext(DialogContext);
  if (!context)
    throw new Error("can't access dialog context outside provider!");
  return context;
};

export const DialogContextProvider: ParentComponent<{}> = (props) => {
  let ref: HTMLDivElement;
  return (
    <>
      <div ref={ref!} />
      <DialogContext.Provider value={{ mount: ref! }}>
        {props.children}
      </DialogContext.Provider>
    </>
  );
};
