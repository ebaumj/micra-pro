import { Navigate, Route } from '@solidjs/router';
import { For, ParentComponent } from 'solid-js';
import { T } from '../../generated/language-types';
import { homeUrl, navigationElements } from './navigationElements';
import { SidebarLayout } from '@micra-pro/shared/ui';

const MenuLayout: ParentComponent = (props) => {
  return (
    <SidebarLayout
      navigationElements={navigationElements}
      bottomElement={{
        icon: 'close',
        link: '/',
        name: () => <T key={'menu-sidebar-exit'} />,
      }}
    >
      {props.children}
    </SidebarLayout>
  );
};

export const MainMenu = () => {
  return (
    <Route path="/menu" component={MenuLayout}>
      <Route path="/" component={() => <Navigate href={homeUrl} />} />
      <For each={navigationElements}>
        {(navElement) => (
          <Route path={navElement.link} component={navElement.component} />
        )}
      </For>
    </Route>
  );
};
