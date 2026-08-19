<script setup>
import { useAuth, logout } from './store/auth'
import { useRouter } from 'vue-router'

const auth = useAuth()
const router = useRouter()

function salir() {
  logout()
  router.push('/login')
}
</script>

<template>
  <nav class="navbar navbar-expand-lg navbar-dark bg-dark" v-if="auth.usuario">
    <div class="container">
      <span class="navbar-brand">Tienda</span>
      <button
        class="navbar-toggler"
        type="button"
        data-bs-toggle="collapse"
        data-bs-target="#navMenu"
      >
        <span class="navbar-toggler-icon"></span>
      </button>
      <div class="collapse navbar-collapse" id="navMenu">
        <ul class="navbar-nav me-auto">
          <li class="nav-item">
            <router-link class="nav-link" to="/productos">Productos</router-link>
          </li>
          <li class="nav-item">
            <router-link class="nav-link" to="/clientes">Clientes</router-link>
          </li>
          <li class="nav-item">
            <router-link class="nav-link" to="/venta">Registrar venta</router-link>
          </li>
          <li class="nav-item">
            <router-link class="nav-link" to="/historial">Historial</router-link>
          </li>
        </ul>
        <span class="navbar-text text-light me-3">Hola, {{ auth.usuario.nombre }}</span>
        <button class="btn btn-outline-light btn-sm" @click="salir">Salir</button>
      </div>
    </div>
  </nav>

  <div class="container py-4">
    <router-view />
  </div>
</template>
