import { Component } from 'solid-js';
import { T } from '../../generated/language-types';
import { EditScalesPage } from '@micra-pro/scale-management/feature';
import { EditBeansPage } from '@micra-pro/bean-management/feature';

export const navigationElements: {
  link: string;
  icon: string;
  name: Component;
  component: Component;
}[] = [
  {
    name: () => <T key={'menu-sidebar-scales'} />,
    link: 'scales',
    icon: 'scale',
    component: EditScalesPage,
  },
  {
    name: () => <T key={'menu-sidebar-beans'} />,
    link: 'beans',
    icon: 'local_cafe',
    component: EditBeansPage,
  },
];

export const homeUrl = navigationElements[0].link;

export default navigationElements;
