import * as signalR from '@microsoft/signalr'
import { useAuthStore } from '../store/useAuthStore'

// In Docker: VITE_API_URL is unset → use relative path, nginx proxies /hubs/ to backend
// In local dev: VITE_API_URL=http://localhost:8081
const HUB_URL = `${import.meta.env.VITE_API_URL || ''}/hubs/notifications`

let connection = null

export function getHubConnection() {
  if (!connection) {
    connection = new signalR.HubConnectionBuilder()
      .withUrl(HUB_URL, {
        accessTokenFactory: () => useAuthStore.getState().accessToken ?? '',
      })
      .withAutomaticReconnect()
      .configureLogging(signalR.LogLevel.Warning)
      .build()
  }
  return connection
}

export async function startHubConnection() {
  const conn = getHubConnection()
  if (conn.state === signalR.HubConnectionState.Disconnected) {
    await conn.start()
  }
}

export async function stopHubConnection() {
  const conn = getHubConnection()
  if (conn.state !== signalR.HubConnectionState.Disconnected) {
    await conn.stop()
  }
}
