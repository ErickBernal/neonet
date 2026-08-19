import { reactive } from 'vue'

const state = reactive({
  usuario: JSON.parse(localStorage.getItem('usuario') || 'null')
})

export function login(nombre) {
  const usuario = { nombre, fecha: new Date().toISOString() }
  localStorage.setItem('usuario', JSON.stringify(usuario))
  state.usuario = usuario
}

export function logout() {
  localStorage.removeItem('usuario')
  state.usuario = null
}

export function useAuth() {
  return state
}
