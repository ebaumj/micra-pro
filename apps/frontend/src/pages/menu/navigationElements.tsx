import { Component } from 'solid-js';
import { T } from '../../generated/language-types';
import { EditBeansPage } from '@micra-pro/bean-management/feature';
import { MainscreenConfigPage } from '../MainscreenConfigPage';
import { BrewByWeightHistoryPage } from '@micra-pro/brew-by-weight/feature';
import { RecipeHubProfilePage } from '@micra-pro/recipe-hub/feature';
import { CleaningPage } from '@micra-pro/cleaning/feature';
import { WifiSetupPage } from '../WifiSetupPage';
import { UpdatesPage } from '../UpdatesPage';

export const navigationElements: {
  link: string;
  icon: string;
  name: Component;
  component: Component;
}[] = [
  {
    name: () => <T key={'menu-sidebar-mainscreen-config'} />,
    link: 'mainscreen-config',
    icon: 'grid_view',
    component: MainscreenConfigPage,
  },
  {
    name: () => <T key={'menu-sidebar-beans'} />,
    link: 'beans',
    icon: 'local_cafe',
    component: EditBeansPage,
  },
  {
    name: () => <T key={'menu-sidebar-history'} />,
    link: 'hisotry',
    icon: 'access_time',
    component: BrewByWeightHistoryPage,
  },
  {
    name: () => <T key={'menu-sidebar-profile'} />,
    link: 'profile',
    icon: 'account_circle',
    component: RecipeHubProfilePage,
  },
  {
    name: () => <T key={'menu-sidebar-cleaning'} />,
    link: 'cleaning',
    icon: 'cleaning_services',
    component: CleaningPage,
  },
  {
    name: () => <T key={'menu-sidebar-wifi'} />,
    link: 'wifi',
    icon: 'wifi',
    component: WifiSetupPage,
  },
  {
    name: () => <T key={'menu-sidebar-updates'} />,
    link: 'updates',
    icon: 'deployed_code_update',
    component: UpdatesPage,
  },
];

export const homeUrl = navigationElements[0].link;

export default navigationElements;
