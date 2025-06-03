import { MicraProLogo, Icon } from '@micra-pro/shared/ui';
import { A } from '@solidjs/router';
import { Component, For, ParentComponent } from 'solid-js';

const SidebarItem: ParentComponent<{
  link: string;
  iconName: string;
  highlightActive: boolean;
}> = (props) => {
  return (
    <A
      class="flex items-center rounded-lg px-2 py-1 hover:bg-secondary active:bg-secondary"
      activeClass={props.highlightActive ? 'bg-secondary' : ''}
      href={props.link}
    >
      <Icon
        iconName={props.iconName}
        class="mr-2 shrink-0 align-middle text-3xl font-extralight leading-none"
      />
      <span class="truncate py-1 text-base font-light leading-none">
        {props.children}
      </span>
    </A>
  );
};

export type NavigationElement = {
  link: string;
  icon: string;
  name: Component;
};

export type SidebarLayoutProps = {
  logoUrl: string;
  navigationElements: NavigationElement[];
  bottomElement: NavigationElement;
};

export const SidebarLayout: ParentComponent<SidebarLayoutProps> = (props) => {
  return (
    <div class="flex h-full max-h-full w-full">
      <div class="left-0 flex h-full max-h-full w-60 shrink-0 flex-col gap-2 border-r">
        <A href={props.logoUrl} class="m-2 w-fit">
          <MicraProLogo class="mx-2 w-14 text-lime-400" />
        </A>
        <ul class="m-4 flex grow flex-col gap-2 overflow-auto">
          <For each={props.navigationElements}>
            {(navElement) => (
              <li>
                <SidebarItem
                  link={navElement.link}
                  iconName={navElement.icon}
                  highlightActive={true}
                >
                  <navElement.name />
                </SidebarItem>
              </li>
            )}
          </For>
          <li class="mt-auto">
            <SidebarItem
              link={props.bottomElement.link}
              iconName={props.bottomElement.icon}
              highlightActive={false}
            >
              <props.bottomElement.name />
            </SidebarItem>
          </li>
        </ul>
      </div>
      <div class="h-full max-h-full grow overflow-auto p-6">
        {props.children}
      </div>
    </div>
  );
};
