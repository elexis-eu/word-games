import Vue from 'vue'
import Router from 'vue-router'

Vue.use(Router)

/* Layout */
import Layout from '@/layout'

/**
 * Note: sub-menu only appear when route children.length >= 1
 * Detail see: https://panjiachen.github.io/vue-element-admin-site/guide/essentials/router-and-nav.html
 *
 * hidden: true                   if set true, item will not show in the sidebar(default is false)
 * alwaysShow: true               if set true, will always show the root menu
 *                                if not set alwaysShow, when item has more than one children route,
 *                                it will becomes nested mode, otherwise not show the root menu
 * redirect: noRedirect           if set noRedirect will no redirect in the breadcrumb
 * name:'router-name'             the name is used by <keep-alive> (must set!!!)
 * meta : {
    roles: ['admin','editor']    control the page roles (you can set multiple roles)
    title: 'title'               the name show in sidebar and breadcrumb (recommend set)
    icon: 'svg-name'             the icon show in the sidebar
    breadcrumb: false            if set false, the item will hidden in breadcrumb(default is true)
    activeMenu: '/example/list'  if set path, the sidebar will highlight the path you set
  }
 */

/**
 * constantRoutes
 * a base page that does not have permission requirements
 * all roles can be accessed
 */
export const constantRoutes = [
  {
    path: '/login',
    component: () => import('@/views/login/index'),
    hidden: true
  },

  {
    path: '/404',
    component: () => import('@/views/404'),
    hidden: true
  },

  {
    path: '/',
    component: Layout,
    redirect: '/dashboard',
    children: [{
      path: 'dashboard',
      name: 'Dashboard',
      component: () => import('@/views/dashboard/index'),
      meta: { title: 'Dashboard', icon: 'dashboard' }
    }]
  },

  
  {
    path: '/users',
    component: Layout,
    redirect: '/users/list',
    name: 'Users',
    meta: { title: 'Users', icon: 'users' },
    children: [
      {
        path: 'list',
        name: 'listUser',
        component: () => import('@/views/users/list'),
        meta: { title: 'List users', icon: 'users' }
      },
      {
        path: 'add',
        name: 'addUser',
        component: () => import('@/views/users/edit'),
        meta: { title: 'Add user', icon: 'user-edit' },
        params: {id: null}
      },
      {
        path: 'edit/:id',
        name: 'editUser',
        component: () => import('@/views/users/edit'),
        meta: { title: 'Edit user', icon: 'user-edit' },
        hidden: true
      }
    ]
  },

  {
    path: '/imports',
    component: Layout,
    redirect: '/imports/list',
    name: 'Imports',
    meta: { title: 'Imports', icon: 'upload-solid' },
    children: [
      {
        path: 'list',
        name: 'listImports',
        component: () => import('@/views/imports/list'),
        meta: { title: 'List imports', icon: 'table' }
      },
      {
        path: 'add',
        name: 'addImport',
        component: () => import('@/views/imports/add'),
        meta: { title: 'Make import', icon: 'plus' },
        params: {id: null}
      },
      {
        path: 'detail/:id',
        name: 'detailImports',
        component: () => import('@/views/imports/detail'),
        meta: { title: 'Detail import', icon: 'user-edit' },
        hidden: true
      }
    ]
  },

  {
    path: '/exports',
    component: Layout,
    redirect: '/exports/list',
    name: 'Exports',
    meta: { title: 'Exports', icon: 'file-export-solid' },
    children: [
      {
        path: 'list',
        name: 'listExports',
        component: () => import('@/views/exports/list'),
        meta: { title: 'List exports', icon: 'table' }
      },
      {
        path: 'add',
        name: 'addImport',
        component: () => import('@/views/exports/add'),
        meta: { title: 'Make export', icon: 'plus' },
      },
    ]
  },
  {
    path: '/translations',
    component: Layout,
    redirect: '/translations/edit',
    name: 'Translations',
    meta: { title: 'Translations', icon: 'file-export-solid' },
    children: [
      {
        path: 'edit',
        name: 'editTranslation',
        component: () => import('@/views/translations/edit'),
        meta: { title: 'Edit translation', icon: 'table' },
      },
    ]
  },
  {
    path: '/structures',
    component: Layout,
    redirect: '/structures/list',
    name: 'Structure',
    meta: { title: 'Structure', icon: 'wrench' },
    children: [
      {
        path: 'list',
        name: 'listStructures',
        component: () => import('@/views/structures/list'),
        meta: { title: 'List structures', icon: 'wrench' }
      },
      {
        path: 'edit',
        name: 'editStructures',
        component: () => import('@/views/structures/edit'),
        meta: { title: 'Edit structure', icon: 'table' },
        hidden: true
      },
      {
        path: 'add',
        name: 'addStructures',
        component: () => import('@/views/structures/edit'),
        meta: { title: 'Add structure', icon: 'plus' },
      },
    ]
  },
  {
    path: '/gamemodes',
    component: Layout,
    redirect: '/gamemodes/edit',
    name: 'Game Modes',
    meta: { title: 'Game Modes', icon: 'swatchbook-solid' },
    children: [
      {
        path: 'edit',
        name: 'editGamemodes',
        component: () => import('@/views/gamemodes/edit'),
        meta: { title: 'Edit gamemodes', icon: 'swatchbook-solid' },
        hidden: true
      },
    ]
  },


  /*
  {
    path: '/example',
    component: Layout,
    redirect: '/example/table',
    name: 'Example',
    meta: { title: 'Example', icon: 'example' },
    children: [
      {
        path: 'table',
        name: 'Table',
        component: () => import('@/views/table/index'),
        meta: { title: 'Table', icon: 'table' }
      },
      {
        path: 'tree',
        name: 'Tree',
        component: () => import('@/views/tree/index'),
        meta: { title: 'Tree', icon: 'tree' }
      }
    ]
  },

  {
    path: '/form',
    component: Layout,
    children: [
      {
        path: 'index',
        name: 'Form',
        component: () => import('@/views/form/index'),
        meta: { title: 'Form', icon: 'form' }
      }
    ]
  },

  {
    path: '/nested',
    component: Layout,
    redirect: '/nested/menu1',
    name: 'Nested',
    meta: {
      title: 'Nested',
      icon: 'nested'
    },
    children: [
      {
        path: 'menu1',
        component: () => import('@/views/nested/menu1/index'), // Parent router-view
        name: 'Menu1',
        meta: { title: 'Menu1' },
        children: [
          {
            path: 'menu1-1',
            component: () => import('@/views/nested/menu1/menu1-1'),
            name: 'Menu1-1',
            meta: { title: 'Menu1-1' }
          },
          {
            path: 'menu1-2',
            component: () => import('@/views/nested/menu1/menu1-2'),
            name: 'Menu1-2',
            meta: { title: 'Menu1-2' },
            children: [
              {
                path: 'menu1-2-1',
                component: () => import('@/views/nested/menu1/menu1-2/menu1-2-1'),
                name: 'Menu1-2-1',
                meta: { title: 'Menu1-2-1' }
              },
              {
                path: 'menu1-2-2',
                component: () => import('@/views/nested/menu1/menu1-2/menu1-2-2'),
                name: 'Menu1-2-2',
                meta: { title: 'Menu1-2-2' }
              }
            ]
          },
          {
            path: 'menu1-3',
            component: () => import('@/views/nested/menu1/menu1-3'),
            name: 'Menu1-3',
            meta: { title: 'Menu1-3' }
          }
        ]
      },
      {
        path: 'menu2',
        component: () => import('@/views/nested/menu2/index'),
        meta: { title: 'menu2' }
      }
    ]
  },
  */

  /*
  {
    path: 'external-link',
    component: Layout,
    children: [
      {
        path: 'https://panjiachen.github.io/vue-element-admin-site/#/',
        meta: { title: 'External Link', icon: 'link' }
      }
    ]
  },
  */

  // 404 page must be placed at the end !!!
  { path: '*', redirect: '/404', hidden: true }
]

const createRouter = () => new Router({
  // mode: 'history', // require service support
  scrollBehavior: () => ({ y: 0 }),
  routes: constantRoutes
})

const router = createRouter()

// Detail see: https://github.com/vuejs/vue-router/issues/1234#issuecomment-357941465
export function resetRouter() {
  const newRouter = createRouter()
  router.matcher = newRouter.matcher // reset router
}

export default router
