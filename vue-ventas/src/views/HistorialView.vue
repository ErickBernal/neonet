<script setup>
import { ref, onMounted } from 'vue'
import api from '../services/api'

const clientes = ref([])
const clienteId = ref('')
const ventas = ref([])
const cargando = ref(false)
const consultado = ref(false)

onMounted(async () => {
  const { data } = await api.get('/clientes')
  clientes.value = data
})

async function consultarHistorial() {
  if (!clienteId.value) return
  cargando.value = true
  consultado.value = true
  const { data } = await api.get(`/ventas/cliente/${clienteId.value}`)
  ventas.value = data
  cargando.value = false
}
</script>

<template>
  <h3 class="mb-4">Historial de ventas</h3>

  <div class="row mb-4">
    <div class="col-12 col-md-6">
      <label class="form-label">Cliente</label>
      <div class="input-group">
        <select v-model="clienteId" class="form-select">
          <option value="" disabled>Selecciona un cliente</option>
          <option v-for="c in clientes" :key="c.id" :value="c.id">
            {{ c.nombre }}
          </option>
        </select>
        <button class="btn btn-primary" @click="consultarHistorial">Buscar</button>
      </div>
    </div>
  </div>

  <div v-if="cargando" class="text-muted">Cargando...</div>

  <div v-else-if="consultado && ventas.length === 0" class="alert alert-info">
    Cliente sin ventas registradas.
  </div>

  <div v-else class="vstack gap-3">
    <div v-for="v in ventas" :key="v.id" class="card shadow-sm">
      <div class="card-body">
        <div class="d-flex justify-content-between mb-2">
          <span class="text-muted small">
            {{ new Date(v.fecha).toLocaleString() }}
          </span>
          <span class="fw-bold">Total: Q{{ v.total.toFixed(2) }}</span>
        </div>
        <ul class="list-unstyled mb-0 small">
          <li v-for="d in v.detalles" :key="d.id">
            {{ d.cantidad }} × {{ d.productoNombre }}
            — Q{{ (d.cantidad * d.precioUnitario).toFixed(2) }}
          </li>
        </ul>
      </div>
    </div>
  </div>
</template>
