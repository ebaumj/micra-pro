import { MicraProLogo } from './MicraProLogo';
import { Icon } from './Icon';
import { A } from '@solidjs/router';
import { Component, For, ParentComponent } from 'solid-js';

const SidebarItem: ParentComponent<{
  link: string;
  iconName: string;
  highlightActive: boolean;
}> = (props) => {
  return (
    <A
      class="active:bg-secondary flex items-center rounded-lg px-2 py-1"
      activeClass={props.highlightActive ? 'bg-secondary' : ''}
      href={props.link}
    >
      <Icon
        iconName={props.iconName}
        class="mr-2 shrink-0 align-middle text-3xl leading-none font-extralight"
      />
      <span class="truncate py-1 text-base leading-none font-light">
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
  navigationElements: NavigationElement[];
  bottomElement: NavigationElement;
};

export const SidebarLayout: ParentComponent<SidebarLayoutProps> = (props) => {
  return (
    <div class="flex h-full max-h-full w-full">
      <div class="left-0 flex h-full max-h-full w-60 shrink-0 flex-col gap-2 border-r">
        <div class="mx-2 mt-2 w-fit pt-2 pl-2">
          <MicraProLogo class="h-14 w-14" />
        </div>
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
