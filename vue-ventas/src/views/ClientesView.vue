<script setup>
import { ref, onMounted } from 'vue'
import api from '../services/api'

const clientes = ref([])
const cargando = ref(true)
const error = ref('')
const mensaje = ref('')
const guardando = ref(false)

const form = ref({ nombre: '', email: '' })

async function cargarClientes() {
  cargando.value = true
  try {
    const { data } = await api.get('/clientes')
    clientes.value = data
  } catch (e) {
    error.value = 'No se pudo conectar con la API.'
  } finally {
    cargando.value = false
  }
}

onMounted(cargarClientes)

async function agregarCliente() {
  error.value = ''
  mensaje.value = ''

  if (!form.value.nombre.trim() || !form.value.email.trim()) {
    error.value = 'Nombre y email son obligatorios'
    return
  }

  guardando.value = true
  try {
    await api.post('/clientes', {
      nombre: form.value.nombre.trim(),
      email: form.value.email.trim()
    })
    mensaje.value = 'Cliente agregado correctamente.'
    form.value = { nombre: '', email: '' }
    await cargarClientes()
  } catch (e) {
    error.value = e.response?.data?.mensaje || 'Error al agregar el cliente.'
  } finally {
    guardando.value = false
  }
}
</script>

<template>
  <h3 class="mb-4">Clientes</h3>

  <div class="row g-4">
    <!-- Formulario -->
    <div class="col-12 col-lg-5">
      <div class="card shadow-sm">
        <div class="card-body">
          <h6 class="card-title">Agregar cliente</h6>

          <form @submit.prevent="agregarCliente">
            <div class="mb-3">
              <label class="form-label">Nombre</label>
              <input v-model="form.nombre" type="text" class="form-control" placeholder="Ej: Ana Gómez" />
            </div>
            <div class="mb-3">
              <label class="form-label">Email</label>
              <input v-model="form.email" type="email" class="form-control" placeholder="ana@correo.com" />
            </div>

            <div v-if="error" class="alert alert-danger py-2">{{ error }}</div>
            <div v-if="mensaje" class="alert alert-success py-2">{{ mensaje }}</div>

            <button type="submit" class="btn btn-primary w-100" :disabled="guardando">
              {{ guardando ? 'Guardando...' : 'Agregar cliente' }}
            </button>
          </form>
        </div>
      </div>
    </div>

    <div class="col-12 col-lg-7">
      <h6>Clientes registrados</h6>
      <div v-if="cargando" class="text-muted">Cargando...</div>
      <ul v-else class="list-group">
        <li
          v-for="c in clientes"
          :key="c.id"
          class="list-group-item d-flex justify-content-between align-items-center"
        >
          <span>{{ c.nombre }}</span>
          <span class="text-muted small">{{ c.email }}</span>
        </li>
      </ul>
    </div>
  </div>
</template>
