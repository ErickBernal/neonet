<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { login } from '../store/auth'

const nombre = ref('')
const router = useRouter()

function ingresar() {
  if (!nombre.value.trim()) return
  login(nombre.value.trim())
  router.push('/productos')
}
</script>

<template>
  <div class="d-flex justify-content-center align-items-center" style="min-height: 80vh;">
    <div class="card shadow-sm" style="width: 24rem;">
      <div class="card-body p-4">
        <h4 class="card-title mb-3 text-center">Ingrese su nombre</h4>

        <form @submit.prevent="ingresar">
          <div class="mb-3">
            <label class="form-label">Nombre</label>
            <input
              v-model="nombre"
              type="text"
              class="form-control"
              placeholder="Ej: Ana Gómez"
              required
            />
          </div>
          <button type="submit" class="btn btn-primary w-100">Entrar</button>
        </form>
      </div>
    </div>
  </div>
</template>
