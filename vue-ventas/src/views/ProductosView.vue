<script setup>
import { ref, onMounted } from 'vue'
import api from '../services/api'

const productos = ref([])
const cargando = ref(true)
const error = ref('')
const mensaje = ref('')
const guardando = ref(false)
const mostrarForm = ref(false)

const form = ref({ nombre: '', precio: '', stock: '' })

async function cargarProductos() {
  cargando.value = true
  try {
    const { data } = await api.get('/productos')
    productos.value = data
  } catch (e) {
    error.value = 'No se pudo conectar con la API.'
  } finally {
    cargando.value = false
  }
}

onMounted(cargarProductos)

async function agregarProducto() {
  error.value = ''
  mensaje.value = ''

  if (!form.value.nombre.trim() || form.value.precio === '' || form.value.stock === '') {
    error.value = 'Nombre, precio y stock son obligatorios'
    return
  }

  guardando.value = true
  try {
    await api.post('/productos', {
      nombre: form.value.nombre.trim(),
      precio: Number(form.value.precio),
      stock: Number(form.value.stock)
    })
    mensaje.value = 'Producto agregado correctamente.'
    form.value = { nombre: '', precio: '', stock: '' }
    await cargarProductos()
  } catch (e) {
    error.value = e.response?.data?.mensaje || 'Error al agregar el producto.'
  } finally {
    guardando.value = false
  }
}
</script>

<template>
  <div class="d-flex justify-content-between align-items-center mb-4">
    <h3 class="mb-0">Productos disponibles</h3>
    <button class="btn btn-outline-primary btn-sm" @click="mostrarForm = !mostrarForm">
      {{ mostrarForm ? 'Cancelar' : '+ Agregar producto' }}
    </button>
  </div>

  <!-- Formulario -->
  <div class="card shadow-sm mb-4" v-if="mostrarForm">
    <div class="card-body">
      <h6 class="card-title">Nuevo producto</h6>
      <form @submit.prevent="agregarProducto" class="row g-3">
        <div class="col-12 col-md-6">
          <label class="form-label">Nombre</label>
          <input v-model="form.nombre" type="text" class="form-control" placeholder="Ej: Monitor 24''" />
        </div>
        <div class="col-6 col-md-3">
          <label class="form-label">Precio</label>
          <input v-model="form.precio" type="number" step="0.01" min="0" class="form-control" />
        </div>
        <div class="col-6 col-md-3">
          <label class="form-label">Stock</label>
          <input v-model="form.stock" type="number" min="0" class="form-control" />
        </div>

        <div class="col-12" v-if="error">
          <div class="alert alert-danger py-2 mb-0">{{ error }}</div>
        </div>
        <div class="col-12" v-if="mensaje">
          <div class="alert alert-success py-2 mb-0">{{ mensaje }}</div>
        </div>

        <div class="col-12">
          <button type="submit" class="btn btn-primary" :disabled="guardando">
            {{ guardando ? 'Guardando...' : 'Guardar producto' }}
          </button>
        </div>
      </form>
    </div>
  </div>

  <div v-if="cargando" class="text-muted">Cargando...</div>
  <div v-else-if="error && productos.length === 0" class="alert alert-danger">{{ error }}</div>

  <div v-else class="row g-3">
    <div class="col-12 col-sm-6 col-lg-4" v-for="p in productos" :key="p.id">
      <div class="card h-100 shadow-sm">
        <div class="card-body">
          <h5 class="card-title">{{ p.nombre }}</h5>
          <div class="d-flex justify-content-between align-items-center mt-2">
            <span class="fw-bold">Q{{ p.precio.toFixed(2) }}</span>
            <span class="badge" :class="p.stock > 0 ? 'bg-success' : 'bg-secondary'">
              Stock: {{ p.stock }}
            </span>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>
