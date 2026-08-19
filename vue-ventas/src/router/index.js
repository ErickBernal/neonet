import { createRouter, createWebHistory } from 'vue-router'
import { useAuth } from '../store/auth'

import LoginView from '../views/LoginView.vue'
import ProductosView from '../views/ProductosView.vue'
import ClientesView from '../views/ClientesView.vue'
import VentaView from '../views/VentaView.vue'
import HistorialView from '../views/HistorialView.vue'

const routes = [
  { path: '/', redirect: '/productos' },
  { path: '/login', name: 'login', component: LoginView },
  { path: '/productos', name: 'productos', component: ProductosView, meta: { requiereAuth: true } },
  { path: '/clientes', name: 'clientes', component: ClientesView, meta: { requiereAuth: true } },
  { path: '/venta', name: 'venta', component: VentaView, meta: { requiereAuth: true } },
  { path: '/historial', name: 'historial', component: HistorialView, meta: { requiereAuth: true } }
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

router.beforeEach((to) => {
  const auth = useAuth()
  if (to.meta.requiereAuth && !auth.usuario) {
    return { name: 'login' }
  }
})

export default router
