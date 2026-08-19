<script setup>
import { ref, computed, onMounted } from 'vue'
import api from '../services/api'

const clientes = ref([])
const productos = ref([])
const clienteId = ref('')
const carrito = ref([]) 
const mensaje = ref('')
const error = ref('')
const enviando = ref(false)

const total = computed(() =>
  carrito.value.reduce((acc, item) => acc + item.precio * item.cantidad, 0)
)

onMounted(async () => {
  const [resClientes, resProductos] = await Promise.all([
    api.get('/clientes'),
    api.get('/productos')
  ])
  clientes.value = resClientes.data
  productos.value = resProductos.data
})

function agregarAlCarrito(producto) {
  const existente = carrito.value.find(i => i.productoId === producto.id)
  if (existente) {
    existente.cantidad++
  } else {
    carrito.value.push({
      productoId: producto.id,
      nombre: producto.nombre,
      precio: producto.precio,
      cantidad: 1
    })
  }
}

function quitarDelCarrito(productoId) {
  carrito.value = carrito.value.filter(i => i.productoId !== productoId)
}

async function registrarVenta() {
  error.value = ''
  mensaje.value = ''

  if (!clienteId.value) {
    error.value = 'Selecciona un cliente'
    return
  }
  if (carrito.value.length === 0) {
    error.value = 'Agrega al menos un producto al carrito'
    return
  }

  enviando.value = true
  try {
    await api.post('/ventas', {
      clienteId: Number(clienteId.value),
      items: carrito.value.map(i => ({
        productoId: i.productoId,
        cantidad: i.cantidad
      }))
    })
    mensaje.value = 'Venta registrada correctamente.'
    carrito.value = []
    clienteId.value = ''
  } catch (e) {
    error.value = e.response?.data?.mensaje || 'Error al registrar la venta.'
  } finally {
    enviando.value = false
  }
}
</script>

<template>
  <h3 class="mb-4">Registrar venta</h3>

  <div class="row g-4">
    <!-- Columna izquierda: cliente + productos -->
    <div class="col-12 col-lg-7">
      <div class="mb-3">
        <label class="form-label">Cliente</label>
        <select v-model="clienteId" class="form-select">
          <option value="" disabled>Selecciona un cliente</option>
          <option v-for="c in clientes" :key="c.id" :value="c.id">
            {{ c.nombre }} ({{ c.email }})
          </option>
        </select>
      </div>

      <h6>Productos</h6>
      <div class="list-group">
        <div
          v-for="p in productos"
          :key="p.id"
          class="list-group-item d-flex justify-content-between align-items-center"
        >
          <div>
            <div class="fw-semibold">{{ p.nombre }}</div>
            <small class="text-muted">Q{{ p.precio.toFixed(2) }} · Stock: {{ p.stock }}</small>
          </div>
          <button class="btn btn-sm btn-outline-primary" @click="agregarAlCarrito(p)">
            Agregar
          </button>
        </div>
      </div>
    </div>

    <!-- Columna derecha: carrito -->
    <div class="col-12 col-lg-5">
      <div class="card shadow-sm">
        <div class="card-body">
          <h6 class="card-title">Carrito</h6>

          <div v-if="carrito.length === 0" class="text-muted small">
            No has agregado productos.
          </div>

          <ul class="list-group list-group-flush mb-3">
            <li
              v-for="item in carrito"
              :key="item.productoId"
              class="list-group-item d-flex justify-content-between align-items-center px-0"
            >
              <div>
                {{ item.nombre }}
                <input
                  type="number"
                  min="1"
                  v-model.number="item.cantidad"
                  class="form-control form-control-sm d-inline-block ms-2"
                  style="width: 4.5rem;"
                />
              </div>
              <div class="text-end">
                <div>Q{{ (item.precio * item.cantidad).toFixed(2) }}</div>
                <button
                  class="btn btn-sm btn-link text-danger p-0"
                  @click="quitarDelCarrito(item.productoId)"
                >
                  quitar
                </button>
              </div>
            </li>
          </ul>

          <div class="d-flex justify-content-between fw-bold mb-3">
            <span>Total</span>
            <span>Q{{ total.toFixed(2) }}</span>
          </div>

          <div v-if="error" class="alert alert-danger py-2">{{ error }}</div>
          <div v-if="mensaje" class="alert alert-success py-2">{{ mensaje }}</div>

          <button
            class="btn btn-primary w-100"
            :disabled="enviando"
            @click="registrarVenta"
          >
            {{ enviando ? 'Enviando...' : 'Registrar venta' }}
          </button>
        </div>
      </div>
    </div>
  </div>
</template>
